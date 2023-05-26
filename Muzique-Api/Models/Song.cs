using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Muzique_Api.Models
{
    public class Song
    {
        public int songId { get; set; }
        public string name { get; set; }
        public string nameSearch { get; set; }
        public string audioUrl { get; set; }
        public string description { get; set; }
        public string coverImageUrl { get; set; }
        public string? lyric { get; set; }
        public int? albumId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get;set; }
        public string? albumname { get; set; }
    }

    public class SongViewModel
    {
        public List<Song> ListSong { get; set; }
        public int TotalRes { get; set; }
    }

    public class SongModel : Song
    {
        public int[] listArtist { get; set; }
        public int[] listArtistDelete { get; set; }
        public int[] listGenre  { get; set; }
        public int[] listGenreDelete { get; set; }
        public int[] listPlaylist { get; set; }
        public int[] listPlaylistDelete { get; set; }
    }

    public class SongDetail
    {
        public Song Song { get; set; }
        public List<int> listArtistId { get; set; }
        public List<int> listGenreId { get; set; }
        public List<int> listPlaylistId { get; set; }
    }
}
