using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AmericanDadEpisodeFixer {
    class Program {
        private static Regex _episodeRegex = new Regex("(?:S(?<Season>\\d?\\d)E(?<Episode>\\d?\\d)|ad(?<Season>\\d)(" + "?<Episode>\\d?\\d))", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static List<EpisodeInfo> _episodes;

        static void Main(string[] args) {
            _episodes = LoadEpisodeInfo();

            var path = args[0];

            var seasonFolders = Directory.GetDirectories(path);

            foreach(var seasonPath in seasonFolders) {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Checking season {seasonPath}");

                var episodeFolders = Directory.GetDirectories(seasonPath);
                foreach(var episodeFolder in episodeFolders) {

                    var episodeFile = Directory.GetFiles(episodeFolder).FirstOrDefault(f => f.EndsWith(".avi") || f.EndsWith(".mkv"));
                    if(episodeFile == null)
                        continue;

                    var file = new FileInfo(episodeFile);

                    var match = _episodeRegex.Match(file.Name);

                    if(!match.Success)
                        continue;

                    var episodeNumber = match.Groups["Episode"].Value.ToInt32();
                    var seasonNumber = match.Groups["Season"].Value.ToInt32();


                    var episodeTargetPath = GetEpisodeTargetPath(seasonNumber, episodeNumber);

                    if(episodeTargetPath == null)
                        continue;

                    var targetPath = Path.Combine(path, episodeTargetPath);

                    if(!Directory.Exists(targetPath))
                        Directory.CreateDirectory(targetPath);

                    if(episodeFolder == targetPath) {
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  Moving episode S{seasonNumber}E{episodeNumber} {file.Name}");
                    Console.WriteLine($"    {targetPath}");

                    foreach(string dirPath in Directory.GetDirectories(episodeFolder, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(episodeFolder, targetPath));

                    foreach(string filePath in Directory.GetFiles(episodeFolder, "*.*", SearchOption.AllDirectories)) {
                        var newPath = filePath.Replace(episodeFolder, targetPath);
                        if(File.Exists(newPath))
                            File.Delete(newPath);

                        File.Move(filePath, filePath.Replace(episodeFolder, targetPath));
                    }
                }
            }
        }

        static string GetEpisodeTargetPath(Int32 season, Int32 episode) {
            var epInfo = _episodes.FirstOrDefault(e => e.Season == season && e.EpisodeNumber == episode);
            if(epInfo == null)
                return null;

            return $"American.Dad.S{epInfo.PlexSeason}-ADFixed\\American.Dad.S{epInfo.PlexSeason}E{epInfo.PlexEpisodeNumber}-ADFixed";
        }

        static List<EpisodeInfo> LoadEpisodeInfo() {
            var doc = XDocument.Load("episodes.xml");

            return doc.Descendants("episode").Select(n => new EpisodeInfo {
                EpisodeNumber = n.Attributes("number").FirstValueOrDefault().ToInt32(),
                Season = n.Parent.Attributes("number").FirstValueOrDefault().ToInt32(),
                PlexEpisodeNumber = n.Attributes("plexNumber").FirstValueOrDefault().ToInt32(),
                PlexSeason = n.Attributes("plexSeason").FirstValueOrDefault().ToInt32()
            }).ToList();
        }
    }
}
