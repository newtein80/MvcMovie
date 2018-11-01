using System;

// 클래스나 속성에 선언적으로 지정할 수 있는 내장 유효성 검사 어트리뷰트들의 모음은 DataAnnotations 네임스페이스에서 제공
// 내장 유효성 검사 어트리뷰트들의 모음과 함께 서식 관련 어트리뷰트들을 제공
// DataType 어트리뷰트는 뷰 엔진에게 단지 데이터의 서식을 지정하기 위한 힌트만 알려줄 뿐입니다
// (추가적으로 URL에 대한 <a> 태그나 전자메일에 대한 <a href="mailto:EmailAddress.com"> 태그를 생성해주는 등의 어트리뷰트도 제공해줍니다.)
using System.ComponentModel.DataAnnotations;

// 불필요한 using 구문들은 밝은 회색 글씨로 구분되어 나타납니다.
// Movie.cs 파일의 아무 곳이나 마우스 오른쪽 버튼으로 클릭한 다음, Usings 구성(Organize Usings) > 불필요한 Using 제거(Remove Unnecessary Usings)를 선택

// Entity Framework Code First를 이용해서 데이터베이스를 자동으로 생성하면,
// Code First가 생성된 데이터베이스 스키마와 그에 대응하는 모델 클래스가 서로 일치하는지 여부를 지속적으로 추적하기 위한 테이블을 데이터베이스에 함께 추가합니다.
// 그리고 그 이후부터 데이터베이스 스키마와 모델 클래스가 일치하지 않으면 Entity Framework가 예외를 던집니다.
// 결과적으로 혹시라도 런타임에 뒤늦게 발견될 수도 있는 (모호한 오류로 인한) 문제점을 미리 개발 시점에 손쉽게 파악할 수 있습니다.
namespace MvcMovie.Models
{
    /// <summary>
    /// Data/Migrations/ 폴더에 Migration 에 대한 아리그레이션 파일이 생성됨!!!!!!!!!!!!
    /// </summary>
    public class Movie
    {
        public int ID { get; set; }

        // StringLength 어트리뷰트는 문자열 속성의 최대 길이를 지정하며, 필요한 경우 선택적으로 최소 길이를 지정할 수도 있습니다.
        // 값 형식들은 (decimal, int, float, DateTime 등) 기본적으로 값이 존재해야만 유효한 것으로 간주되므로 [Required] 어트리뷰트를 명시적으로 지정하지 않아도 동일한 효과를 갖습니다.
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        //Display 어트리뷰트는 필드의 이름으로 출력될 문자열을 지정
        [Display(Name = "Release Date")]
        //DataType 어트리뷰트는 필드 데이터의 형식을 지정하는데, 날짜 형식을 지정했으므로 필드에 저장된 시간 정보가 출력되지 않습니다.
        //일반적으로 DataType 어트리뷰트는 데이터베이스의 내장 형식보다 더 구체적인 데이터 서식을 지정하기 위해서 사용되며, 유효성 검사 어트리뷰트가 아닙니다.
        //DataType 열거형은 Date, Time, PhoneNumber, Currency, EmailAddress 등 다양한 데이터 형식들을 제공해줍니다.
        /*
         * DataType 어트리뷰트를 이용해서 응용 프로그램이 자동으로 특정 데이터 형식에 최적화된 기능을 제공하도록 활성화시킬 수도 있습니다.
         * 예를 들어서 DataType.EmailAddress 열거형이 지정되면 자동으로 mailto: 링크가 생성되고, HTML5를 지원하는 브라우저에서는 DataType.Date 열거형이 지정되면 날짜 선택기가 제공됩니다.
         * DataType 어트리뷰트가 HTML 5를 지원하는 브라우저가 인식 가능한 HTML 5 data- ("데이터 대시"라고 읽습니다) 어트리뷰트를 만들어내기 때문입니다.
         *
         * 그러나 다시 한 번 강조하지만 DataType 어트리뷰트는 어떠한 유효성 검사 기능도 제공해주지 않습니다.
         *
         * DataType.Date 열거형 값이 출력되는 날짜의 서식 그 자체를 지정하는 것은 아니라는 점에 주의하시기 바랍니다.
         * 기본적으로 날짜 필드는 서버의 CultureInfo에 기반한 기본 서식에 따라서 출력됩니다.
         *
         * 날짜의 서식을 명시적으로 지정하기 위해서는 DisplayFormat 어트리뷰트를 사용하면 됩니다
         *
         * DataType 어트리뷰트는 데이터를 화면에 렌더하는 직접적인 방법이 아닌,
         * 데이터의 의미(Semantics)를 전달해주며 다음과 같이 DisplayFormat 어트리뷰트가 지원하지 않는 이점들을 제공해줍니다
         * 1. 브라우저에서 HTML5 기능을 사용할 수 있습니다. (달력 컨트롤 지원, 지역에 적합한 통화 기호, 이메일 링크, 일부 클라이언트 측 입력 유효성 검사 등)
         * 2. 기본적으로 브라우저가 자체적으로 로케일에 따라 올바른 서식으로 데이터를 렌더합니다.
         * 3. DataType 어트리뷰트를 사용하면 MVC가 데이터를 렌더할 올바른 필드 템플릿을 선택하는데 도움이 됩니다.
         * (DisplayFormat 어트리뷰트는 일반적인 문자열 템플릿을 사용합니다)
         * https://bradwilson.typepad.com/blog/2009/10/aspnet-mvc-2-templates-part-1-introduction.html
         * DateTime 형식에 Range 어트리뷰트를 적용하면 jQuery 유효성 검사 기능이 정상적으로 동작하지 않습니다.
         * 가령 다음 코드는 입력된 날짜가 지정한 범위 내에 존재하더라도 항상 클라이언트 측 유효성 검사 오류가 출력됩니다.
         *
         * DateTime 형식에 대해 Range 어트리뷰트를 사용하는 것은 권장되지 않습니다.
         * */
        [DataType(DataType.Date)]
        // ApplyFormatInEditMode 속성은 편집 텍스트 상자에 값이 출력될 때도 지정된 서식을 적용하도록 지시합니다.
        // (그러나 필드에 따라서는 이런 기능이 불필요한 경우도 있습니다. 예를 들어서, 통화 값의 경우 편집 텍스트 상자에까지 통화 기호가 함께 나타나는 것을 원하는 경우는 많지 않습니다.)
        // DisplayFormat 어트리뷰트만 단독으로 사용하는 것보다는 DataType 어트리뷰트를 함께 사용하는 것이 바람직합니다.
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        // RegularExpression 어트리뷰트는 입력 가능한 문자를 제한하기 위한 용도로 사용됩니다.
        // 이를테면, 이번 예제 코드에서 Genre 필드와 Rating 필드에는 반드시 문자만 입력할 수 있습니다 (공백, 숫자 또는 특수 문자는 허용되지 않습니다).
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        // Required 어트리뷰트나 MinimumLength 속성이 설정된 StringLength 어트리뷰트가 지정된 속성은 반드시 값이 입력되야만 유효한 것으로 인식되는데,
        // 사용자가 이 유효성 검사를 통과하기 위해서 공백을 입력하는 것까지 제한하지는 못합니다.
        [Required]
        [StringLength(30)]
        public string Genre { get; set; }

        // Range 어트리뷰트는 지정된 속성에 입력할 수 있는 값을 특정 범위 내로 제한합니다.
        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Movie 클래스에 새로운 필드가 추가되었으므로 바인딩 화이트 리스트에도 이 새로운 필드를 추가해야 합니다.
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(5)]
        public string Rating { get; set; }

        /*
         *
         * 다음과 같이 한 줄로 표현 할 수 있다.
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "Release Date"), DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$"), Required, StringLength(30)]

        public string Genre { get; set; }

        [Range(1, 100), DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$"), StringLength(5)]

        public string Rating { get; set; }
         * */
    }
}