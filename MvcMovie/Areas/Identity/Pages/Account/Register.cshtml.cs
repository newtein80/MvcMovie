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
        // ����ڰ� �� �׸���� �Է��ϰ� Register ��ũ�� Ŭ���ϸ�, Account ��Ʈ�ѷ��� UserManager ���񽺿� SignInManager ���񽺰� ���Ե˴ϴ�:
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

        // ��� ��ũ�� Ŭ���ϸ� ���� ������ ȣ��
        // Register �׼ǿ��� UserManager ��ü�� CreateAsync �޼��尡 ȣ��Ǿ� ����ڰ� �����˴ϴ�:
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                // The user is created by CreateAsync on the _userManager object. _userManager is provided by dependency injection)
                // ����ڰ� ����
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
                    // ����: ����� ��� ��� �α����� �����ϴ� ����� ���� Ȯ�� �� �����Ͻñ� �ٶ��ϴ�.
                    // https://docs.microsoft.com/ko-kr/aspnet/core/security/authentication/accconfirm?view=aspnetcore-2.1&tabs=visual-studio#prevent-login-at-registration
                    // SignInAsync �޼���� SignInManager Ŭ�������� �����Ǵ� �޼���
                    // �ʿ��� ���, ��Ʈ�ѷ� �׼� ������ �α����� ������� �ſ� ���� ������ ������ ���� �ֽ��ϴ�.
                    // ���� ��, HomeController.Index �׼� �޼��忡 �ߴ����� �����ϰ� User.claims �Ӽ��� ���캸�� �� ���� ������ Ȯ���� �� �ֽ��ϴ�.
                    // �̷��� ����ڰ� �α����� �ϰ� ���� ������ ������ �ο��� �� �ִµ�, ���Ѻο��� ���� ���� �ڼ��� ������ ���Ѻο� �������� �����Ͻñ� �ٶ��ϴ�.
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
