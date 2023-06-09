﻿using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin, User")]
        public IActionResult Get(int page, int rowperpage, string? keyword = "")
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
        [Authorize(Roles = "Admin, User")]
        public IActionResult GetSongById(int id)
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
        [Authorize(Roles = "Admin, User")]
        public IActionResult GetSongDetail(int id)
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
        [Authorize(Roles = "Admin")]
        public IActionResult Post(SongModel model)
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

                        song.name = model.name;
                        song.description = model.description;
                        song.audioUrl = model.audioUrl;
                        song.coverImageUrl = model.coverImageUrl;
                        song.albumId = model.albumId;
                        song.createdAt = DateTime.Now;
                        song.nameSearch = model.nameSearch;
                        song.lyric = model.lyric;
                        if (!songService.InsertSong(song, transaction)) return StatusCode(500, "Lỗi khi thêm bài hát");

                        int songId = songService.GetLastSongID(transaction);

                        if (model.listArtist != null)
                        {
                            foreach (var id in model.listArtist)
                            {
                                ArtistService artistService = new ArtistService(connect);
                                Artist artist = artistService.GetArtistDetail(id, transaction);
                                if (artist == null) return NotFound();

                                SongAndArtist songAndArtist = new SongAndArtist();
                                songAndArtist.artistId = id;
                                songAndArtist.songId = songId;
                                songAndArtist.createdAt = DateTime.Now;

                                if (!songService.InsertSongArtist(songAndArtist, transaction)) return StatusCode(500, "Lỗi ca sĩ");

                            }
                        }

                        if (model.listPlaylist != null)
                        {
                            foreach (var id in model.listPlaylist)
                            {
                                PlaylistService playlistService = new PlaylistService(connect);
                                Playlist playlist = playlistService.GetPlaylistById(id, transaction);
                                if (playlist == null) return NotFound();

                                SongAndPlaylist songAndPlaylist = new SongAndPlaylist();
                                songAndPlaylist.playlistId = id;
                                songAndPlaylist.songId = songId;
                                songAndPlaylist.createdAt = DateTime.Now;

                                if (!songService.InsertSongPlaylist(songAndPlaylist, transaction)) return StatusCode(500, "Lỗi playlist");
                            }
                        }

                        if (model.listGenre != null)
                        {
                            foreach (var id in model.listGenre)
                            {
                                GenreService genreService = new GenreService(connect);
                                Genre genre = genreService.GetGenreDetail(id, transaction);
                                if (genre == null) return NotFound();

                                SongAndGenre songAndGenre = new SongAndGenre();
                                songAndGenre.genreId = id;
                                songAndGenre.songId = songId;
                                songAndGenre.createdAt = DateTime.Now;

                                if (!songService.InsertSongGenre(songAndGenre, transaction)) return StatusCode(500, "Lỗi thể loại");
                            }
                        }

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

        [HttpPut("/updateSong")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSong(SongModel model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        SongService songService = new SongService(connect);
                        Song song = songService.GetSongById(model.songId, transaction);
                        if (song == null) return StatusCode(500, "Bài hát không tồn tại");

                        song.name = model.name;
                        song.nameSearch = model.nameSearch;
                        song.description = model.description;
                        song.updatedAt = DateTime.Now;
                        song.albumId = model.albumId;
                        song.lyric = song.lyric;

                        if (!string.IsNullOrEmpty(model.coverImageUrl))
                        {
                            await _deleteFile.DeleteFileAsync(song.coverImageUrl);

                            song.coverImageUrl = model.coverImageUrl;
                        }

                        if (!string.IsNullOrEmpty(model.audioUrl))
                        {
                            await _deleteFile.DeleteFileAsync(song.audioUrl);

                            song.audioUrl = model.audioUrl;
                        }

                        if (model.listArtist.Length > 0)
                        {
                            foreach (var id in model.listArtist)
                            {
                                ArtistService artistService = new ArtistService(connect);
                                Artist artist = artistService.GetArtistDetail(id, transaction);
                                if (artist == null) return NotFound();

                                SongAndArtist songAndArtist = new SongAndArtist();
                                songAndArtist.artistId = id;
                                songAndArtist.songId = model.songId;
                                songAndArtist.createdAt = DateTime.Now;

                                if (!songService.InsertSongArtist(songAndArtist, transaction)) return StatusCode(500, "Lỗi khi sửa ca sĩ");

                            }
                        }

                        if (model.listArtistDelete.Length > 0)
                        {
                            foreach (var id in model.listArtistDelete)
                            {
                                ArtistService artistService = new ArtistService(connect);
                                Artist artist = artistService.GetArtistDetail(id, transaction);
                                if (artist == null) return NotFound();

                                if (!artistService.DeleteArtistSong(id, transaction)) return StatusCode(500, "Lỗi khi xoá ca sĩ");

                            }
                        }

                        if (model.listPlaylist.Length > 0)
                        {
                            foreach (var id in model.listPlaylist)
                            {
                                PlaylistService playlistService = new PlaylistService(connect);
                                Playlist playlist = playlistService.GetPlaylistById(id, transaction);
                                if (playlist == null) return NotFound();

                                SongAndPlaylist songAndPlaylist = new SongAndPlaylist();
                                songAndPlaylist.playlistId = id;
                                songAndPlaylist.songId = model.songId;
                                songAndPlaylist.createdAt = DateTime.Now;

                                if (!songService.InsertSongPlaylist(songAndPlaylist, transaction)) return StatusCode(500, "Lỗi sửa playlist");
                            }
                        }

                        if (model.listPlaylistDelete.Length > 0)
                        {
                            foreach (var id in model.listPlaylistDelete)
                            {
                                PlaylistService playlistService = new PlaylistService(connect);
                                Playlist playlist = playlistService.GetPlaylistById(id, transaction);
                                if (playlist == null) return NotFound();

                                if (!playlistService.DeletePlaylistSong(id, transaction)) return StatusCode(500, "Lỗi xoá playlist");
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
                                songAndGenre.songId = model.songId;
                                songAndGenre.createdAt = DateTime.Now;

                                if (!songService.InsertSongGenre(songAndGenre, transaction)) return StatusCode(500, "Lỗi sửa thể loại");
                            }
                        }

                        if (model.listGenreDelete.Length > 0)
                        {
                            foreach (var id in model.listGenreDelete)
                            {
                                GenreService genreService = new GenreService(connect);
                                Genre genre = genreService.GetGenreDetail(id, transaction);
                                if (genre == null) return NotFound();

                                if (!genreService.DeleteGenreSong(id, transaction)) return StatusCode(500, "Lỗi xoá thể loại");
                            }
                        }

                        if (!songService.UpdateSong(song,transaction)) return StatusCode(500, "Lỗi khi sửa Bài hát");

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

        [HttpDelete("/deleteSong")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        SongService songService = new SongService(connect);

                        Song song = songService.GetSongById(id, transaction);
                        if (song == null) return NotFound();

                        SongDetail songDetail = songService.GetSongDetail(id, transaction);

                        if (!songService.DeleteSongArtist(id, transaction)) return StatusCode(500, "Lỗi khi xoá ca sĩ");
                        if (!songService.DeleteSongGenre(id, transaction)) return StatusCode(500, "Lỗi khi xoá thể loại");
                        if (songDetail.listPlaylistId.Count > 0)
                        {
                            if (!songService.DeleteSongPlaylist(id, transaction)) return StatusCode(500, "Lỗi khi xoá playlist");
                        }

                        await _deleteFile.DeleteFileAsync(song.coverImageUrl);
                        await _deleteFile.DeleteFileAsync(song.audioUrl);

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
