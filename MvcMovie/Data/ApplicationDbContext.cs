using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data
{
    /// <summary>
    /// ApplicationDbContext 클래스는 데이터베이스의 연결, Movie 개체와 데이터베이스 레코드 간의 매핑 작업 등을 처리
    /// 데이터베이스 컨텍스트는 Startup.cs 파일의 ConfigureServices 메서드에서 의존성 주입(Dependency Injection) 컨테이너를 통해서 등록
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MvcMovie.Models.Movie> Movie { get; set; }
    }
}
