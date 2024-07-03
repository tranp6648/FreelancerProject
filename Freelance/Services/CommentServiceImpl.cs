using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PhinaMart.Models;
using PhinaMart.Services;
using PhinaMart.ViewModels;
using System.Security.Claims;

public class CommentServiceImpl : CommentService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly PhinaMartContext _phinaMartContext;

    public CommentServiceImpl(IHttpContextAccessor contextAccessor, PhinaMartContext phinaMartContext)
    {
        _contextAccessor = contextAccessor;
        _phinaMartContext = phinaMartContext;
    }

    public bool CreateComment(CreateComment createComment, int id)
    {
        try
        {
            var Id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                throw new Exception("User ID claim not found");
            }
            var UserId = int.Parse(Id.Value);

            {
                var Comment = new Comment
                {
                    UserId = UserId,
                    ProductId = id,
                    CreatedAt = DateTime.Now,
                    CommentText=createComment.Comment_Text,
                };
                _phinaMartContext.Comments.Add(Comment);
            }
            return _phinaMartContext.SaveChanges() > 0;
        }
        catch
        {
            return false;
        }
    }
    public bool DeleteCompare(int id)
    {
        try
        {
            var Id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                throw new Exception("User ID claim not found");
            }
            var UserId = int.Parse(Id.Value);
            var Compare = _phinaMartContext.Compares.FirstOrDefault(d => d.IdProduct == id && d.IdUser == UserId);
            if (Compare != null)
            {
                _phinaMartContext.Compares.Remove(Compare);
            }
            return _phinaMartContext.SaveChanges() > 0;
        }
        catch
        {
            return false;
        }
    }
    public bool CreateCompare(int id)
    {
        try
        {
            var Id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                throw new Exception("User ID claim not found");
            }
            var UserId = int.Parse(Id.Value);
            var Compare = new Compare
            {
                IdProduct = id,
                IdUser = UserId,
            };
            _phinaMartContext.Compares.Add(Compare);
            return _phinaMartContext.SaveChanges()>0;
        }
        catch
        {
            return false;
        }
      
    }
}
