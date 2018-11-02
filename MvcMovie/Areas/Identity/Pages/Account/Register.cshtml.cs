using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MvcMovie.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        // 사용자가 각 항목들을 입력하고 Register 링크를 클릭하면, Account 컨트롤러에 UserManager 서비스와 SignInManager 서비스가 주입됩니다:
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        // 등록 링크를 클립하면 다음 동작이 호출
        // Register 액션에서 UserManager 개체의 CreateAsync 메서드가 호출되어 사용자가 생성됩니다:
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                // The user is created by CreateAsync on the _userManager object. _userManager is provided by dependency injection)
                // 사용자가 생성
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // If the user was created successfully, the user is logged in by the call to _signInManager.SignInAsync
                    // 참고: 사용자 등록 즉시 로그인을 방지하는 방법은 계정 확인 을 참고하시기 바랍니다.
                    // https://docs.microsoft.com/ko-kr/aspnet/core/security/authentication/accconfirm?view=aspnetcore-2.1&tabs=visual-studio#prevent-login-at-registration
                    // SignInAsync 메서드는 SignInManager 클래스에서 제공되는 메서드
                    // 필요한 경우, 컨트롤러 액션 내에서 로그인한 사용자의 신원 세부 정보에 접근할 수도 있습니다.
                    // 예를 들어서, HomeController.Index 액션 메서드에 중단점을 설정하고 User.claims 속성을 살펴보면 그 세부 정보를 확인할 수 있습니다.
                    // 이렇게 사용자가 로그인을 하고 나면 적절한 권한을 부여할 수 있는데, 권한부여에 대한 보다 자세한 정보는 권한부여 문서들을 참고하시기 바랍니다.
                    // http://www.egocube.pe.kr/Translation/Index/asp-net-core-security
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
