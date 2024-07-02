using PhinaMart.ViewModels;

namespace PhinaMart.Services
{
    public interface CommentService
    {
        public bool CreateComment(CreateComment createComment,int id);
        public bool CreateCompare(int id);
    }
}
