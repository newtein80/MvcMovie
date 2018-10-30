using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers
{
    public class HelloWorldController : Controller
    {
        // 
        // GET: /HelloWorld/
        /// <summary>
        /// URL에 "/HelloWorld/"를 추가해서 호출할 수 있는 HTTP GET 메서드임
        /// 컨트롤러의 메서드는 (액션 메서드, Action Methods)
        /// 문자열 같은 기본 형식(Primitive Types) 대신 IActionResult나 ActionResult로부터 파생된 클래스를 반환
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //시스템에게 HTML을 반환하도록 요청했으며 그에 따라 시스템이 응답을 한 것입니다.
            //MVC는 전달 받은 URL에 근거하여 컨트롤러 클래스를 (그리고 그 내부의 액션 메서드를) 호출
            //Razor 기반의 뷰 템플릿은 .cshtml 이라는 파일 확장자
            //View 개체를 반환
            //return View(); 구문을 실행함으로써 브라우저로 전송할 응답을 뷰 템플릿 파일을 이용해서 렌더하도록 지시
            //(사용해야 할 뷰 템플릿 파일의 이름을 명시적으로 지정하지 않았기 때문에, MVC는 기본적으로 /Views/HelloWorld 폴더에 위치한 Index.cshtml 뷰 파일을 사용)
            //메뉴 레이아웃은 Views/Shared/_Layout.cshtml 파일에 구현
            return View();
        }

        // 
        // GET: /HelloWorld/Welcome/ 
        /// <summary>
        /// URL에 "/HelloWorld/Welcome/"를 추가해서 호출할 수 있는 HTTP GET 메서드임
        /// numTimes 매개변수에 아무런 값도 전달되지 않았을 경우 기본값으로 1이 사용되도록 이 매개변수에 C#의 선택적 매개변수(Optional-Parameter) 기능이 사용
        /// </summary>
        /// <param name="name"></param>
        /// <param name="numTimes"></param>
        /// <returns></returns>
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            //악의적인 입력으로부터 응용 프로그램을 보호하기 위해서 HtmlEncoder.Default.Encode를 사용 가능
            //응답을 생성하기 위해서 필요한 간단한 데이터를 컨트롤러에서 뷰로 전달
            //뷰 템플릿에 필요한 동적 데이터(매개변수들)를 컨트롤러 내부에서 ViewData 사전에 설정한 다음, 이를 뷰 템플릿에서 접근
            //ViewData 사전은 동적 개체로, 원하는 것은 무엇이든 설정할 수 있으며, 여러분이 ViewData 개체에 무언가 설정하기 전까지는 아무런 속성도 정의되어 있지 않습니다.
            //MVC 모델 바인딩 시스템이 자동으로 브라우저 주소 표시줄의 쿼리 문자열에 포함된 명명된 매개변수들(name과 numTimes)을 액션 메서드의 매개변수에 매핑
            //URL로부터 얻어진 데이터가 모델 바인더에 의해서 컨트롤러로 전달됩니다.
            //컨트롤러는 이 데이터를 ViewData 사전에 정리해서 담고 이 개체를 다시 뷰로 전달합니다.
            //마지막으로 뷰는 해당 데이터를 HTML로 렌더해서 이를 브라우저에 전달
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;

            return View();
        }
    }
}