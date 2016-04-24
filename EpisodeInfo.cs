using System;

namespace AmericanDadEpisodeFixer {
    public class EpisodeInfo {
        public Int32 Season { get; set; }
        public Int32 EpisodeNumber { get; set; }
        public Int32 PlexSeason { get; set; }
        public Int32 PlexEpisodeNumber { get; set; }

        public override string ToString() {
            return $"S{Season}E{EpisodeNumber} --> S{PlexSeason}E{PlexEpisodeNumber}";
        }
    }
}