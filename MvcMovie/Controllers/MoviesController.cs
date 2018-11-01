using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        //컨트롤러의 생성자에서는 의존성 주입(Dependency Injection)을 이용해서 테이터베이스 컨텍스트를 컨트롤러에 주입하고 있습니다. 데이터베이스 컨텍스트는 컨트롤러의 각 CRUD 메서드들에서 사용
        /// <summary>
        /// 의존성 주입(Dependency Injection)을 이용해서 테이터베이스 컨텍스트를 컨트롤러에 주입
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// MoviesController 컨트롤러의 생성자
        /// </summary>
        /// <param name="context"></param>
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Movie.ToListAsync());
        //}

        //public async Task<IActionResult> Index(string searchString)
        //{
        //    var movies = from m in _context.Movie
        //                 select m;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        movies = movies.Where(s => s.Title.Contains(searchString));
        //    }

        //    return View(await movies.ToListAsync());
        //}

        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            // Use LINQ to get list of genre's.
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var movies = from m in _context.Movie
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel();
            movieGenreVM.genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            movieGenreVM.movies = await movies.ToListAsync();

            return View(movieGenreVM);
        }

        // GET: Movies/Details/5
        /// <summary>
        /// localhost:1234/movies/details/1이라는 URL을 요청할 경우:
        /// 컨트롤러에는 Movies 컨트롤러가(첫 번째 URL 세그먼트)
        /// 액션 메서드로는 details 메서드가(두 번째 URL 세그먼트)
        /// id 매개변수에는 1이 설정됩니다(마지막 URL 세그먼트).
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Movie 모델의 인스턴스가 Details 뷰로 전달</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Entity Framework의 FirstOrDefaultAsync 메서드로 정보를 조회, 조회한 영화 정보를 Edit 뷰에 전달
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            //Movie 모델의 인스턴스가 Details 뷰로 전달
            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [Bind] 어트리뷰트
        // 오버포스팅(Over-Posting) 공격을 방지할 수 있는 한 가지 방법입니다.
        // 반드시 [Bind] 어트리뷰트에는 변경을 허용하려는 속성들만 포함시켜야 합니다.
        // [HttpPost] 어트리뷰트
        // 메서드가 오직 POST 요청에 의해서만 호출되도록 지정 ([HttpGet] 어트리뷰트는 기본값)
        // [ValidateAntiForgeryToken] 어트리뷰트
        // Edit 뷰 파일에서(Views/Movies/Edit.cshtml) 생성되는 위조 방지 토큰(Anti-Forgery Token)과 한 쌍으로 크로스 사이트 요청 위조(Cross-Site Request Forgery)를 방지하기 위한 용도
        // 위조 방지 토큰은 Edit 뷰 파일에서 Form 태그 헬퍼(Form Tag Helper)에 의해서 생성
        // Form 태그 헬퍼는 숨겨진 위조 방지 토큰을 생성하는데, 반드시 그 값과 Movies 컨트롤러의 Edit 메서드에 지정된 [ValidateAntiForgeryToken] 어트리뷰트가 생성하는 위조 방지 토큰이 일치해야만 합니다. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (id != movie.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.ID == id);
        }
    }
}
