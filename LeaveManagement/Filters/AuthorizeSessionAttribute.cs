using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeaveManagement.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        public string[] Roles { get; set; }

        public AuthorizeSessionAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (Roles != null && Roles.Length > 0)
            {
                var roleId = context.HttpContext.Session.GetInt32("RoleId");
                var roleName = GetRoleName(roleId);

                if (!Roles.Contains(roleName))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private string GetRoleName(int? roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Manager",
                3 => "Employee",
                _ => "Unknown"
            };
        }
    }
}
