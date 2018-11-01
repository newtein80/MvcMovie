using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        //public async Task<IActionResult> Index(string id)
        //{
        //    var movies = from m in _context.Movie
        //                 select m;

        //    if (!String.IsNullOrEmpty(id))
        //    {
        //        movies = movies.Where(s => s.Title.Contains(id));
        //    }

        //    return View(await movies.ToListAsync());
        //}

        // Index 메서드는 개체들의 목록을 조회한 다음), 그 개체를 (모델을) 뷰로 전달
        // Index 메서드의 [HttpPost] 오버로드 버전은 존재하지 않습니다.
        // 이 메서드는 데이터를 필터링하기만 할 뿐, 응용 프로그램의 어떠한 상태도 변경하지 않기 때문에 [HttpPost] 오버로드 버전은 필요가 없습니다.
        public async Task<IActionResult> Index(string movieGenre, string id)
        {
            // Use LINQ to get list of genre's.
            // 데이터베이스에서 모든 장르들을 조회하는 LINQ 쿼리
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            // 이 시점에는 단지 쿼리가 정의 되었을 뿐, 실제로 데이터베이스를 대상으로 실행되는 것은 아닙니다.
            var movies = from m in _context.Movie
                         select m;

            // id 매개변수에 검색할 문자열이 담겨 있으면, 다음 코드를 이용해서 검색 문자열 값으로 필터를 설정해서 movies 쿼리를 변경
            if (!String.IsNullOrEmpty(id))
            {
                // s => s.Title.Contains() 구문은 람다 식(Lambda Expression)
                // 메서드 기반의 LINQ 쿼리에서는 람다가 이 코드에 사용된 Where 메서드나 Contains 메서드 같은 표준 쿼리 연산자 메서드의 인자로 사용
                // LINQ 쿼리는 해당 쿼리가 정의되는 시점이나, Where나 Contains, 또는 OrderBy 같은 메서드 호출에 의해 쿼리가 변경되는 시점에는 실행되지 않습니다.
                // 쿼리의 실제 값이 루프문 등에서 구체적으로 접근되거나, ToListAsync 같은 특정 메서드가 호출될 때까지 표현식의 평가가 미뤄지고 쿼리 실행이 지연됩니다.
                // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/query-execution
                // Contains 메서드는 C# 코드가 아닌 데이터베이스에서 실행됩니다. 그리고 Contains 메서드는 데이터베이스에서 대소문자를 구분하지 않는 SQL LIKE 문으로 매핑됩니다.
                movies = movies.Where(s => s.Title.Contains(id));
            }

            if (!String.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel();
            // 장르들의 SelectList는 Distinct 메서드를 이용해서 중복되지 않는 장르들만 취합해서 만들어집니다 (선택 목록에 같은 장르가 여러 번 나타나는 것을 피하기 위한 작업입니다).
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
        /// <summary>
        /// 초기 Create 폼을 출력
        /// </summary>
        /// <returns>Create 폼</returns>
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
        // Create 메서드에서는 빈 개체를 Create 뷰에 전달
        /// <summary>
        /// 폼 전송을 처리
        /// </summary>
        /// <param name="movie">Movie</param>
        /// <returns>Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            // 제출된 데이터에 유효성 검사 오류가 존재하는지 여부를 검사하기 위해서 ModelState.IsValid를 호출
            // 이 메서드가 호출될 때 개체에 적용된 모든 유효성 검사 어트리뷰트들이 평가
            // 개체에 유효성 검사 오류가 존재하면 Create 메서드가 폼을 다시 출력
            // 클라이언트 측에서 유효성 검사 오류가 감지되면 아예 서버로 폼이 전송되지 않기 때문에, 현재 상태로는 두 번째 Create 메서드가 호출되는 일은 결코 일어나지 않습니다.
            // 브라우저에서 JavaScript를 비활성화시키는 경우에만 클라이언트 측 유효성 검사 자체가 비활성화되어,
            // HTTP POST Create 메서드가 ModelState.IsValid 메서드를 호출함으로써 서버 측에서 영화 정보의 유효성 검사 오류 여부를 확인할 수 있습니다.
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                // 오류가 존재하지 않으면 메서드가 새로운 영화 정보를 데이터베이스에 저장
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        // Visual Studio의 스캐폴딩 시스템은 Edit 뷰를 생성할 때, Movie 클래스를 분석해서 클래스에 존재하는 각 속성들에 대한 <label> 요소와 <input> 요소를 렌더하는 코드를 생성합니다.
        // HTTP GET 메서드에서 데이터 변경 작업을 수행하는 일은, GET 요청에서 응용 프로그램의 상태를 변경하면 안된다고 제시하고 있는 HTTP 권장 사례와 구조적 REST 패턴에 위배
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
        // [ValidateAntiForgeryToken] 어트리뷰트로 Form 태그 헬퍼가 위조 방지 토큰 생성기로 생성한 숨겨진 XSRF 토큰의 유효성을 검사합니다.
        // 모델 바인딩(Model Binding) 시스템이 전송된 폼 값들을 받아서 movie 매개변수에 전달되는 Movie 개체를 생성
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.ID)
            {
                return NotFound();
            }

            // ModelState.IsValid 메서드는 폼으로 제출된 데이터가 Movie 개체를 갱신하는데 적합한지 여부를 확인
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    // 만약 데이터가 유효하다면, 갱신된 영화 데이터가 데이터베이스 컨텍스트의 SaveChangesAsync 메서드 호출을 통해서 데이터베이스에 저장
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

                // 데이터가 저장된 뒤에는 방금 변경한 영화 정보를 비롯한 모든 영화 정보들의 목록이 제공되는 MoviesController 클래스의 Index 액션 메서드로 페이지가 재전송
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