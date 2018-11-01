using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Models;

namespace MvcMovie
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add framework services.
            // ASP.NET Core Configuration 시스템이 ConnectionString을 읽어오는데, 로컬 개발에서는 appsettings.json 파일에서 연결 문자열을 가져옵니다
            // https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/configuration/index?view=aspnetcore-2.1
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            //MVC에 의해서 사용되는 기본 URL 라우팅 로직은 호출할 코드를 결정하기 위해서 다음과 같은 세그먼트 형식을 사용
            // /[Controller]/[ActionName]/[Parameters]
            //아무런 URL 세그먼트로 지정하지 않으면, 기본적으로 아래의 코드에 강조된 템플릿 라인에 정의된 대로 "Home" 컨트롤러의 "Index" 메서드가 지정
            /*
             * 첫 번째 URL 세그먼트는 실행할 컨트롤러 클래스를 결정합니다. 따라서 localhost:xxxx/HelloWorld는 HelloWorldController 클래스에 매핑
             * URL 세그먼트의 두 번째 부분은 클래스의 액션 메서드를 결정
             * localhost:xxxx/HelloWorld/Index는 HelloWorldController 클래스의 Index 메서드를 실행
             * Index 메서드가 명시적으로 메서드 이름이 지정되지 않았을 경우 컨트롤러에서 호출되는 기본 메서드로 지정
             * URL 세그먼트의 세 번째 부분(Parameters)은 라우트 데이터
             * 
             * http://localhost:xxxx/HelloWorld/Welcome로 이동해봅니다. 그러면 Welcome 메서드가 실행되어 문자열이 반환
             * 
             * id 매개변수 뒤에 지정된 ?는 (id?) 이 매개변수가 선택적 매개변수임을 의미
             * */

            /*
             * 1. 상위 폴더 열기(Open Containing Folder)
             * 2. 오른쪽 버튼으로 아무 폴더나 클릭한 다음, 여기서 명령 창 열기(Open command window here)를 선택
             * 3. 명령 프롬프트에서 cd .. 명령을 실행해서 한 단계 위의 프로젝트 디렉터리로 이동
             * 4. 명령 프롬프트에 다음 명령들을 차례대로 실행합니다
             *    dotnet ef migrations add Initial
             *    dotnet ef database update
             * > 명령어 설명
             * dotnet :
             *         (.NET Core)은 크로스-플랫폼을 지원하는 .NET의 구현
             * dotnet ef migrations add Initial :
             *         명령은 Entity Framework .NET Core CLI의 마이그레이션 명령을 실행하고 첫 번째 마이그레이션을 생성합니다.
             *         이 명령에서 "Initial" 대신 다른 인자 값을 이름으로 지정할 수도 있지만,
             *         관례적으로 첫 번째 데이터베이스 마이그레이션에는 "Initial"이라는 이름을 지정하는 경우가 대부분입니다.
             *         이 작업을 수행하면 데이터베이스에 Movie 테이블을 추가하는 (또는 드랍하는) 마이그레이션 명령이 구현된 Data/Migrations/2016<date-time>_Initial.cs 파일이 생성
             * dotnet ef database update : 
             *         명령은 방금 생성한 마이그레이션을 이용해서 데이터베이스를 갱신
             * */
            app.UseMvc(routes =>
            {
                //라우팅의 형식
                // ex. http://localhost:1234/Movies/Edit/4라는 URL을 매개변수 ID 값이 4로 설정된, Movies 컨트롤러의 Edit 액션 메서드에 대한 요청으로 해석
                //컨트롤러의 메서드를 액션 메서드라고 합니다.
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
