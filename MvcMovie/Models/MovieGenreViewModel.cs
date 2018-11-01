using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MvcMovie.Models
{
    /// <summary>
    /// 영화-장르 뷰 모델에는 다음과 같은 정보들이 담기게 됩니다:
    /// 영화 정보들의 목록
    /// 장르들의 목록이 담긴 SelectList 형식의 필드 (이 정보를 이용해서 사용자가 목록에서 장르를 선택할 수 있도록 구현합니다.)
    /// 사용자가 목록에서 선택한 장르를 담는 movieGenre 필드
    /// </summary>
    public class MovieGenreViewModel
    {
        public List<Movie> movies;
        public SelectList genres;
        public string movieGenre { get; set; }
    }
}