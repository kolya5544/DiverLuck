using DiverLuckCore;

namespace DiverLuckInterpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*Test1();
            Test2();
            Test3();
            Test4();
            Test5(); they don't work anymore...
            Test6();*/
            //HelloWorldTest();

            WebServerApp();
        }

        /// <summary>
        /// A program that outputs 15 newlines.
        /// </summary>
        public static void Test1()
        {
            string program = "&(((+++++++++++)+++++++++++++++++++++++++++++++++++++++++)>+).....>(((\\"; // define the function
            program += "))).))).)))."; // call the function three times

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        /// <summary>
        /// A program that creates a primitive integer with a value of 10
        /// </summary>
        public static void Test2()
        {
            string program = "+++++0=@"; // create primitive integer and set its value to 10

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        /// <summary>
        /// A program that creates a primitive integer array
        /// </summary>
        public static void Test3()
        {
            string program = "+++++0=@1";

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        /// <summary>
        /// A program that has several infinite loops
        /// </summary>
        public static void Test4()
        {
            string program = "[[[[+b]b]b]b]";

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        /// <summary>
        /// A program that creates an instance of StringBuilder
        /// </summary>
        public static void Test5()
        {
            string program = "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++)>++++++++++++++++++++++++++)5";

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        /// <summary>
        /// A program that will run until the value of memory cell is equal to 100
        /// </summary>
        public static void Test6()
        {
            string program = "[+ie100f{b}]";

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        public static void HelloWorldTest()
        {
            string program = "[+ie97f{b}])>[+ie28f{b}])<5[+ie255f{b}]"; // create a new StringBuilder
            program += "&>[+ie11f{b}]0[+ie32f{b}]=\\"; // create a function that creates a new CHAR and sets it to SPACEBAR
            program += "&((((((((^[<ie255f{b}]>[-ie0f{b}][+ie25f{b}])))))<.>>((((((((\\"; // create a function that first pushes CHAR into stack, then goes to StringBuilder,
                                                                               // selects Append(char), and puts current CHAR into StringBuilder
            program += ">>((((((((";
            program += "))).[-ie0f{b}][+ie72f{b}]=[-ie0f{b}](+)."; // create "H" (72) and push to SB
            program += "))).[-ie0f{b}][+ie101f{b}]=[-ie0f{b}](+)."; // create "e"
            program += "))).[-ie0f{b}][+ie108f{b}]=[-ie0f{b}](+)."; // create "l"
            program += "))).[-ie0f{b}][+ie108f{b}]=[-ie0f{b}](+)."; // create "l"
            program += "))).[-ie0f{b}][+ie111f{b}]=[-ie0f{b}](+)."; // create "o"
            program += "))).[-ie0f{b}][+ie44f{b}]=[-ie0f{b}](+)."; // create ","
            program += "))).[-ie0f{b}][+ie32f{b}]=[-ie0f{b}](+)."; // create " "
            program += "))).[-ie0f{b}][+ie119f{b}]=[-ie0f{b}](+)."; // create "w"
            program += "))).[-ie0f{b}][+ie111f{b}]=[-ie0f{b}](+)."; // create "o"
            program += "))).[-ie0f{b}][+ie114f{b}]=[-ie0f{b}](+)."; // create "r"
            program += "))).[-ie0f{b}][+ie108f{b}]=[-ie0f{b}](+)."; // create "l"
            program += "))).[-ie0f{b}][+ie100f{b}]=[-ie0f{b}](+)."; // create "d"
            program += "))).[-ie0f{b}][+ie33f{b}]=[-ie0f{b}](+)."; // create "!"
            program += ">[+ie4f{b}])))))[<ie255f{b}]."; // call .ToString()
            program += "(((((((([-ie0f{b}][+ie11f{b}])[+ie52f{b}])[+ie82f{b}])."; // call System.Console.WriteLine(System.String)

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }

        public static void WebServerApp()
        {
            // create a function to create StringBuilder (0)
            string program = "&[ie0f{b}-][+ie97f{b}])[-ie28f{b}])5(((((((\\";
            // create a function to create char (1)
            program += "&[ie0f{b}-][+ie11f{b}]0[+ie32f{b}]=\\";
            // create a function to append char to StringBuilder (2)
            program += "&((((((^<[ie0f{b}-][+ie25f{b}]))))).(((((((\\";
            // create a function to append "index.html" to StringBuilder (3)
            program += "&>#1$[+ie105f{b}]=#2$"; // i
            program += ">#1$[+ie110f{b}]=#2$"; // n
            program += ">#1$[+ie100f{b}]=#2$"; // d
            program += ">#1$[+ie101f{b}]=#2$"; // e
            program += ">#1$[+ie120f{b}]=#2$"; // x
            program += ">#1$[+ie46f{b}]=#2$"; // .
            program += ">#1$[+ie104f{b}]=#2$"; // h
            program += ">#1$[+ie116f{b}]=#2$"; // t
            program += ">#1$[+ie109f{b}]=#2$"; // m
            program += ">#1$[+ie108f{b}]=#2$(((((((\\"; // l
            // create a function to get "index.html" as String (4)
            program += "&[ie0f{b}-][+ie59f{b}])[-ie26f{b}])[-ie4f{b}]))).(((((((\\";
            // create a function to get file contents by String (5)
            program += "&[ie0f{b}-][+ie41f{b}])[-ie13f{b}])[+ie29f{b}]).(((((((\\";
            // create a function to send String over StreamWriter (6) 
            program += "&[ie0f{b}-][+ie41f{b}])[+ie46f{b}])[-ie12f{b}]))).(((((((\\";
            // create a function to receive String over StreamReader (7)
            program += "&[ie0f{b}-][+ie41f{b}])[+ie45f{b}])[-ie12f{b}]))).(((((((\\";
            // create a function to compare String to WHITESPACE (8)
            program += "&[ie0f{b}-][+ie11f{b}])[+ie244f{b}])[-ie152f{b}]).(((((((\\";
            // create a function to create TcpListener on port 8183 (9)
            program += "&[ie0f{b}-][+ie8183f{b}]=^[-ie61f{b}])[-ie29f{b}])[-ie19f{b}]).(((((((\\";
            // create a function to start TcpListener (10)
            program += "&[ie0f{b}-][+ie61f{b}])[-ie29f{b}])[-ie5f{b}]))).(((((((\\";
            // create a function to accept TcpClient (11)
            program += "&[ie0f{b}-][+ie61f{b}])[-ie29f{b}])[-ie10f{b}]))).(((((((\\";
            // create a function to get NetworkStream (12)
            program += "&[ie0f{b}-][+ie61f{b}])[-ie28f{b}])[-ie22f{b}]))).(((((((\\";
            // create a StreamReader over NetworkStream (13)
            program += "&[ie0f{b}-][+ie41f{b}])[+ie45f{b}])5(((((((\\";
            // create a StreamWriter over NetworkStream (14)
            program += "&[ie0f{b}-][+ie41f{b}])[+ie46f{b}])5(((((((\\";
            // create TRUE boolean => set StreamWriter AutoFlush to true (15)
            program += "&>[ie0f{b}-]0+=^<<<<<>>>>[ie0f{b}-][+ie41f{b}])[+ie46f{b}])[ie0f{b}-]))))v(((((((\\";
            // close TcpClient (16)
            program += "&[ie0f{b}-][+ie61f{b}])[-ie28f{b}])[-ie23f{b}]))).(((((((\\";

            // create TcpListner in memory cell 0; var a = TcpListener.Create(8181);
            program += "#9$v^";
            // start TcpListener; a.Start();
            program += "#10$";
            // start accepting TcpClients in a loop; while (true) { var b = a.AcceptTcpClient(); <- cell 1
            program += "[#11$>v^";
            // get NetworkStream; var c = b.GetStream(); <- cell 2
            program += "#12$>v^";
            // get StreamReader; var d = new StreamReader(c); <- cell 3
            program += ">#13$";
            // get StreamWriter; var e = new StreamWriter(c); <- cell 4
            program += "<^>>#14$";
            // set AutoFlush to true; e.AutoFlush = true;
            program += "#15$";
            // read string from StreamReader in a loop; while (true) { var f = d.ReadLine(); <- cell 5
            program += "[<#7$>>v^";
            // check if string is equal to WHITESPACE (\n) or NULL -> put the result into cell 6 -> break if yes
            program += "#8$>v~ie1f{b}<<";
            // end loop; }
            program += "]";
            // load StringBuilder into cell 6
            program += "#0$";
            // load "index.html" into StringBuilder
            program += "#3$";
            // get string "index.html" into cell 6
            program += "#4$v^";
            // get file contents into cell 6
            program += "#5$v^";
            // send file contents over StreamWriter
            program += "<<#6$";
            // close TCP stream
            program += "<<<#16$<";
            // end loop; }
            program += "]";

            File.WriteAllText("program.dlc", program);

            var dl = new DiverLuck();
            dl.ExecuteProgram(program);
        }
    }
}