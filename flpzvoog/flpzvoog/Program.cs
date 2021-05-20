using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad.FLParser;

namespace flpzvoog
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            //Console.WriteLine("Hello World!");
            //Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
            if (args.Length > 0)
            {

                testAll(args[0]);
            }
        }
        static void testAll(string filePath)
        {
            Console.WriteLine("start");
            Console.WriteLine(filePath);
            Project project1 = Project.Load(filePath, false);
            dumpProj(project1);
            Console.WriteLine("end");
        }
        static void dumpProj(Project project)
        {
            Console.WriteLine("MainVolume " + project.MainVolume);
            Console.WriteLine("MainPitch " + project.MainPitch);
            Console.WriteLine("Ppq " + project.Ppq);
            Console.WriteLine("Tempo " + project.Tempo);
            Console.WriteLine("ProjectTitle " + project.ProjectTitle);
            Console.WriteLine("Comment " + project.Comment);
            Console.WriteLine("Author " + project.Author);
            Console.WriteLine("Genre " + project.Genre);
            Console.WriteLine("VersionString " + project.VersionString);
            Console.WriteLine("Version " + project.Version);
            Console.WriteLine("PlayTruncatedNotes " + project.PlayTruncatedNotes);
            Console.WriteLine("Channels " + project.Channels.Count);
            Console.WriteLine("Tracks " + project.Tracks.Length);
            Console.WriteLine("Patterns " + project.Patterns.Count);
            Console.WriteLine("Inserts " + project.Inserts.Length);
            for (int i = 0; i < project.Channels.Count; i++)
            {
                Console.WriteLine("channel " + project.Channels[i].Id + ": " + project.Channels[i].Name);
            }
            for (int i = 0; i < project.Tracks.Length; i++)
            {
                if (project.Tracks[i].Items.Count > 0)
                {
                    Console.WriteLine("track " + i + ": " + project.Tracks[i].Items.Count + ": " + project.Tracks[i].Name);
                    for(int k=0;k< project.Tracks[i].Items.Count; k++) { 
                        Console.WriteLine("     " + k + ": " + project.Tracks[i].Items[k].Position+ ": " + project.Tracks[i].Items[k].Length+ ": " + project.Tracks[i].Items[k].StartOffset);
                        }
                }
            }
        }
    }
}
