using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace gMKVToolNix
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Reads a Process's standard reader stream character by character and calls the user defined method for each line
        /// </summary>
        /// <param name="argProcess"></param>
        /// <param name="argHandler"></param>
        /// <param name="processStream"></param>
        public static void ReadStreamPerCharacter(
            this Process argProcess, 
            Action<Process, string> argLineDataHandler,
            ProcessStream processStream = ProcessStream.StandardOutput)
        {
            StreamReader reader;
            switch (processStream)
            {
                case ProcessStream.StandardInput:
                    throw new InvalidOperationException("Cannot read from StandardInput stream!");
                case ProcessStream.StandardOutput:
                    reader = argProcess.StandardOutput;
                    break;
                case ProcessStream.StandardError:
                    reader = argProcess.StandardError;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(processStream), processStream, null);
            }
            
            StringBuilder line = new StringBuilder();
            while (true)
            {
                if (!reader.EndOfStream)
                {
                    char c = (char)reader.Read();
                    if (c == '\r')
                    {
                        if ((char)reader.Peek() == '\n')
                        {
                            // consume the next character
                            reader.Read();
                        }

                        argLineDataHandler(argProcess, line.ToString());
                        line.Length = 0;
                    }
                    else if (c == '\n')
                    {
                        argLineDataHandler(argProcess, line.ToString());
                        line.Length = 0;
                    }
                    else
                    {
                        line.Append(c);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
