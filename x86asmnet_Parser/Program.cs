using AsmInstructionFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace x86asmnet_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            (new Program()).NonstaticMain(args);
        }

        public void NonstaticMain(string[] args)
        {
            Console.WriteLine("enter geek-opcodes URI:");
            Console.WriteLine("example: http://ref.x86asm.net/geek.html");
            string uri = Console.ReadLine();
            WebClient wc = new WebClient();
            Console.Write("does internet connection require proxy server? (y/n) "); var key = Console.ReadKey(); Console.WriteLine();
            if (key.KeyChar == 'y' || key.KeyChar == 'Y')
            {
                Console.Write("enter proxy address: "); string proxyaddr = Console.ReadLine();
                var proxy = new WebProxy();
                proxy.Address = new Uri(proxyaddr);
                Console.Write("enter username (if left empty - useDefaultCredentials): "); string username = Console.ReadLine();
                if (username != "")
                {
                    Console.Write("enter password: "); string password = Console.ReadLine();
                    Console.Write("enter domain name: "); string domain = Console.ReadLine();
                    proxy.Credentials = new NetworkCredential(username, password, domain);
                }
                else
                    proxy.UseDefaultCredentials = true;
                wc.Proxy = proxy;
            }
            string geekOpcodes = wc.DownloadString(uri);

            List<List<string>> cells = new List<List<string>>();

            Dictionary<int, int> rowspans = new Dictionary<int, int>(); //key is column index, value is pending rowspan length
            Dictionary<int, bool> currsetRowspans = new Dictionary<int, bool>();
            int currColspan = 0;
            List<string> currRow = null;
            string cellContent = null;

            int depthLevel = 0; //0 -> look for table, 1 -> look for tbody, 2 -> look for tr, 3 -> look for td, 4 -> yay you found some content
            for (int i = 0; i < geekOpcodes.Length; i++)
            {
                //basic xml browsing
                if (geekOpcodes[i] == '<')
                {
                    string tagName = PeekTagName(geekOpcodes, i);
                    if (tagName == "")
                    {
                        //skip to closing
                        while (i < geekOpcodes.Length && geekOpcodes[i] != '>') i++;
                        continue;
                    }
                    i += tagName.Length;
                    if (tagName[0] != '/') //opening tag
                    {
                        //get tag content
                        int tagContentStart = i + 1;
                        i = tagContentStart;
                        for (; i < geekOpcodes.Length && geekOpcodes[i] != '>'; i++) ;
                        if (i == geekOpcodes.Length) break;
                        string tagContent = geekOpcodes.Substring(tagContentStart, i - tagContentStart).Trim();
                        bool singleTag = tagContent.EndsWith("/");

                        switch (tagName.ToLower())
                        {
                            case "table":
                                if (depthLevel == 0 && tagContent.IndexOf("class=\"ref_table") != -1) depthLevel++;
                                else if (depthLevel >= 4) { depthLevel++; cellContent = (cellContent ?? "") + "<table " + tagContent + '>'; }
                                break;
                            case "tbody":
                                if (depthLevel == 1) depthLevel++;
                                else if (depthLevel >= 4) { depthLevel++; cellContent = (cellContent ?? "") + "<tbody " + tagContent + '>'; }
                                break;
                            case "tr":
                                if (depthLevel == 2)
                                {
                                    depthLevel++;
                                    //init new row
                                    currRow = new List<string>();
                                }
                                else if (depthLevel >= 4) { depthLevel++; cellContent = (cellContent ?? "") + "<tr " + tagContent + '>'; }
                                break;
                            case "td":
                                if (depthLevel == 3)
                                {
                                    depthLevel++;
                                    //fill preceding cells from rowspans
                                    for (int k = currRow.Count; rowspans.ContainsKey(k) && rowspans[k] > 0 && !currsetRowspans[k]; k++)
                                    {
                                        rowspans[k]--;
                                        currRow.Add(cells[cells.Count - 1][k]);
                                    }
                                    //init current cell
                                    cellContent = "";
                                    //read colspan
                                    if (tagContent.IndexOf("colspan=\"", StringComparison.InvariantCultureIgnoreCase) != -1)
                                    {
                                        int colspanIndex = tagContent.IndexOf("colspan=\"", StringComparison.InvariantCultureIgnoreCase) + "colspan=\"".Length;
                                        currColspan = 0;
                                        for (int j = colspanIndex; j < tagContent.Length && tagContent[j] != '\"'; j++) currColspan = (currColspan * 10) + (tagContent[j] - '0');
                                        currColspan--;
                                    }
                                    else currColspan = 0;
                                    //update rowspans (also basing on cellspans)
                                    if (tagContent.IndexOf("rowspan=\"", StringComparison.InvariantCultureIgnoreCase) != -1)
                                    {
                                        int rowspanIndex = tagContent.IndexOf("rowspan=\"", StringComparison.InvariantCultureIgnoreCase) + "rowspan=\"".Length;
                                        int currRowspan = 0;
                                        for (int j = rowspanIndex; j < tagContent.Length && tagContent[j] != '\"'; j++) currRowspan = (currRowspan * 10) + (tagContent[j] - '0');
                                        currRowspan--;
                                        for (int col = currRow.Count; col <= currRow.Count + currColspan; col++)
                                        {
                                            if (rowspans.ContainsKey(col)) { rowspans[col] = currRowspan; currsetRowspans[col] = true; }
                                            else { rowspans.Add(col, currRowspan); currsetRowspans.Add(col, true); }
                                        }
                                    }
                                }
                                else if (depthLevel >= 4) { depthLevel++; cellContent = (cellContent ?? "") + "<td " + tagContent + '>'; }
                                break;
                            default:
                                if (depthLevel >= 4) { cellContent = (cellContent ?? "") + '<' + tagName + ' ' + tagContent + '>'; }
                                break;
                        }
                    }
                    else //closing tag
                    {
                        switch (tagName.ToLower())
                        {
                            case "/table":
                                if (depthLevel == 1) depthLevel--;
                                else if (depthLevel >= 4) { depthLevel--; cellContent = (cellContent ?? "") + '<' + tagName + '>'; }
                                break;
                            case "/tbody":
                                if (depthLevel == 2)
                                {
                                    depthLevel--;
                                    //drop rowspans
                                    rowspans = new Dictionary<int, int>();
                                    currsetRowspans = new Dictionary<int, bool>();
                                }
                                else if (depthLevel >= 4) { depthLevel--; cellContent = (cellContent ?? "") + '<' + tagName + '>'; }
                                break;
                            case "/tr":
                                if (depthLevel == 3)
                                {
                                    depthLevel--;
                                    //fill row (also basing on rowspans)
                                    for (int k = currRow.Count; k <= (rowspans.Keys.Count() > 0 ? rowspans.Keys.Max() : 0); k++)
                                    {
                                        if (rowspans.ContainsKey(currRow.Count) && rowspans[currRow.Count] > 0 && !currsetRowspans[currRow.Count])
                                        {
                                            rowspans[currRow.Count]--;
                                            currRow.Add(cells[cells.Count - 1][currRow.Count]);
                                        }
                                        else currRow.Add("");
                                    }
                                    //commit row
                                    cells.Add(currRow);
                                    currRow = null;
                                    //clear currsetRowspans
                                    for (int keyIndex = 0; keyIndex < currsetRowspans.Keys.Count; keyIndex++)
                                        currsetRowspans[currsetRowspans.Keys.ElementAt(keyIndex)] = false;
                                }
                                else if (depthLevel >= 4) { depthLevel--; cellContent = (cellContent ?? "") + '<' + tagName + '>'; }
                                break;
                            case "/td":
                                if (depthLevel == 4)
                                {
                                    depthLevel--;
                                    //commit cell
                                    currRow.Add(cellContent);
                                    cellContent = null;
                                    //fill colspan
                                    while (currColspan > 0)
                                    {
                                        if (rowspans.ContainsKey(currRow.Count) && rowspans[currRow.Count] > 0 && !currsetRowspans[currRow.Count])
                                        {
                                            rowspans[currRow.Count]--;
                                            currRow.Add(cells[cells.Count - 1][currRow.Count]);
                                        }
                                        else currRow.Add("");
                                        currColspan--;
                                    }
                                }
                                else if (depthLevel >= 4) { depthLevel--; cellContent = (cellContent ?? "") + '<' + tagName + '>'; }
                                break;
                            default:
                                if (depthLevel >= 4) cellContent = (cellContent ?? "") + '<' + tagName + '>';
                                break;
                        }
                    }
                }
                else if(geekOpcodes[i] != '>')
                {
                    if (depthLevel >= 4) cellContent = (cellContent ?? "") + geekOpcodes[i];
                }
            }
            
            //all data from html parsed, store in own structures
            List<AsmInstruction> instructions = new List<AsmInstruction>();
            foreach (var row in cells)
            {
                string prefixByte = row[0];
                string _0fPrefix = row[1];
                string primaryOpc = row[2];
                string secondaryOpc = row[3];
                string priOpcodeFields = row[4];
                string registerOpcodeExtension = row[5];
                string processorNumber = row[6];
                string documentationStatus = row[7];
                string executionMode = row[8];
                string lockPrefix = row[10];
                string mnemonic = row[11];
                string operand1 = row[12];
                string operand2 = row[13];
                string operand3 = row[14];
                string operand4 = row[15];
                string extensionGroup = row[16];
                string category = (row[17].Trim() + ' ' + row[18].Trim() + ' ' + row[19].Trim()).Trim();
                string testedFlags = row[20];
                string modifiedFlags = row[21];
                string definedFlags = row[22];
                string undefinedFlags = row[23];
                string setFlags = row[24];
                string description = row[25];

                AsmInstruction instr = new AsmInstruction();

                bool instructionInvalid = mnemonic.Contains("invalid");
                bool undefBehavior = mnemonic.Contains("undefined") || mnemonic.Contains("null");
                bool nopLike = mnemonic.Contains("nop") || mnemonic.Contains("null");
                if (mnemonic.Contains("no mnemonic")) mnemonic = "";
                //todo
                //...

                int byteCount = 0;
                long opcode = 0;
                if (prefixByte != "" && IsHexNumber(prefixByte))
                {
                    opcode |= HexStringToLong(prefixByte) << (8 * byteCount);
                    byteCount++;
                }
                //todo
                //...
            }
        }

        public string PeekTagName(string xml, int start)
        {
            int end = start;
            for (; end < xml.Length; end++)
            {
                if (end == start && xml[end] == '<') { start++; continue; }
                if (end == start && xml[end] == '/') continue;
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".IndexOf(xml[end]) == -1) break;
            }

            if (start == end) return "";
            return xml.Substring(start, end - start);
        }

        public bool IsHexNumber(string s)
        {
            s = s.ToLower();
            string hexDigits = "0123456789abcdef";
            for (int i = 0; i < s.Length; i++)
            {
                if (hexDigits.IndexOf(s[i]) == -1)
                    return false;
            }
            return true;
        }

        public long HexStringToLong(string s)
        {
            long buf = 0;
            s = s.ToLower();
            string hexDigits = "0123456789abcdef";

            for (int i = 0; i < s.Length && i < 16; i++)
            {
                if (hexDigits.IndexOf(s[i]) != -1)
                    buf = buf * 16 + hexDigits.IndexOf(s[i]);
                else
                    throw new FormatException();
            }
            return buf;
        }
    }
}
