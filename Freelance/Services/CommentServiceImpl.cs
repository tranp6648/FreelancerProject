using PhinaMart.Models;
using PhinaMart.ViewModels;
using System.Security.Claims;

namespace PhinaMart.Services
{
    public class CommentServiceImpl : CommentService
    {
        private readonly PhinaMartContext phinaMartContext;
        private readonly IHttpContextAccessor _contextAccessor;
        public CommentServiceImpl(IHttpContextAccessor contextAccessor,PhinaMartContext phinaMartContext)
        {
            _contextAccessor = contextAccessor;
            this.phinaMartContext = phinaMartContext;
        }
        public bool CreateComment(CreateComment createComment,int id)
        {
            try
            {
                var Id=_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (Id == null)
                {
                    throw new Exception("User ID claim not found");
                }
                var UserId=int.Parse(Id.Value);
                if (createComment.Rating == null)
                {
                    var Comment = new Comment
                    {
                        UserId = UserId,
                        ProductId = id,
                        CreatedAt=DateTime.Now,

                    };
                    phinaMartContext.Comments.Add(Comment);
                }
                else
                {
                    var Comment = new Comment
                    {
                        UserId = UserId,
                        ProductId = id,
                        CreatedAt = DateTime.Now,
                        Rating = createComment.Rating
                    };
                    phinaMartContext.Comments.Add(Comment);
                }
                return phinaMartContext.SaveChanges()>0;
               
            }
            catch
            {
                return false;
            }
        }
    }
}
