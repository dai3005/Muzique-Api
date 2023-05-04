using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;
using Muzique_Api.Services;
using System.Diagnostics;
using System.Net;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private DeleteFile _deleteFile;

        public SongController(IWebHostEnvironment env)
        {
            _env = env;
            _deleteFile = new DeleteFile(_env);
        }

        [HttpGet("/getListSong")]
        public ActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                SongService songService = new SongService();

                return Ok(songService.GetListSong(page, rowperpage, keyword));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getSongById")]
        public ActionResult GetSongById(string id)
        {
            try
            {
                SongService songService = new SongService();

                return Ok(songService.GetSongById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getSongDetail")]
        public ActionResult GetSongDetail(string id)
        {
            try
            {
                SongService songService = new SongService();

                return Ok(songService.GetSongDetail(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("/createSong")]
        public ActionResult Post(SongModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        SongService songService = new SongService(connect);

                        Song song = new Song();
                        song.songId = Guid.NewGuid().ToString();
                        song.name = model.name;
                        song.description = model.description;
                        song.audioUrl = model.audioUrl;
                        song.coverImageUrl = model.coverImageUrl;
                        song.albumId = model.albumId;
                        song.createdAt = DateTime.Now;
                        song.nameSearch = model.nameSearch;

                        if (model.listArtist.Length > 0)
                        {
                            foreach (var id in model.listArtist)
                            {
                                ArtistService artistService = new ArtistService(connect);
                                Artist artist = artistService.GetArtistDetail(id, transaction);
                                if (artist == null) return NotFound();

                                SongAndArtist songAndArtist = new SongAndArtist();
                                songAndArtist.artistId = id;
                                songAndArtist.songId = song.songId;
                                songAndArtist.createdAt = DateTime.Now;

                                if (!songService.InsertSongArtist(songAndArtist, transaction)) return StatusCode(500,"Lỗi ca sĩ");
                                
                            }
                        }

                        if (model.listPlaylist.Length > 0)
                        {
                            foreach(var id in model.listPlaylist)
                            {
                                PlaylistService playlistService = new PlaylistService(connect);
                                Playlist playlist = playlistService.GetPlaylistDetail(id, transaction);
                                if (playlist == null) return NotFound();

                                SongAndPlaylist songAndPlaylist = new SongAndPlaylist();
                                songAndPlaylist.playlistId = id;
                                songAndPlaylist.songId = song.songId;
                                songAndPlaylist.createdAt = DateTime.Now;

                                if (!songService.InsertSongPlaylist(songAndPlaylist, transaction)) return StatusCode(500, "Lỗi playlist");
                            }
                        }

                        if (model.listGenre.Length > 0)
                        {
                            foreach (var id in model.listGenre)
                            {
                                GenreService genreService = new GenreService(connect);
                                Genre genre = genreService.GetGenreDetail(id, transaction);
                                if (genre == null) return NotFound();

                                SongAndGenre songAndGenre = new SongAndGenre();
                                songAndGenre.genreId = id;
                                songAndGenre.songId = song.songId;
                                songAndGenre.createdAt = DateTime.Now;

                                if (!songService.InsertSongGenre(songAndGenre, transaction)) return StatusCode(500, "Lỗi thể loại");
                            }
                        }

                        if(!songService.InsertSong(song, transaction)) return StatusCode(500,"Lỗi khi thêm bài hát");

                        transaction.Commit();

                        return Ok();
                    }
                }       
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/deleteSong")]
        public ActionResult Delete(string id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        SongService songService = new SongService(connect);

                        Song song = songService.GetSongById(id,transaction);
                        if (song == null) return NotFound();

                        if (!songService.DeleteSongArtist(id, transaction)) return StatusCode(500, "Lỗi khi xoá ca sĩ");
                        if (!songService.DeleteSongGenre(id, transaction)) return StatusCode(500, "Lỗi khi xoá thể loại");
                        if (!songService.DeleteSongPlaylist(id, transaction)) return StatusCode(500, "Lỗi khi xoá playlist");

                        _deleteFile.DeleteFileAsync(song.coverImageUrl);
                        _deleteFile.DeleteFileAsync(song.audioUrl);

                        if (!songService.DeleteSong(id, transaction)) return StatusCode(500, "Lỗi khi xoá bài hát");

                        transaction.Commit();
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
