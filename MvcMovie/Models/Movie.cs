using System;
using System.ComponentModel.DataAnnotations;
//불필요한 using 구문들은 밝은 회색 글씨로 구분되어 나타납니다.
//Movie.cs 파일의 아무 곳이나 마우스 오른쪽 버튼으로 클릭한 다음, Usings 구성(Organize Usings) > 불필요한 Using 제거(Remove Unnecessary Usings)를 선택

namespace MvcMovie.Models
{
    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }

        //Display 어트리뷰트는 필드의 이름으로 출력될 문자열을 지정
        [Display(Name = "Release Date")]
        //DataType 어트리뷰트는 필드 데이터의 형식을 지정하는데, 날짜 형식을 지정했으므로 필드에 저장된 시간 정보가 출력되지 않습니다.
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
    }
}
