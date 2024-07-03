using PhinaMart.ViewModels;

namespace PhinaMart.Services
{
    public interface CommentService
    {
        public bool CreateComment(CreateComment createComment,CreateRating createRating,int id);
        public bool CreateCompare(int id);
<<<<<<< HEAD
        public bool DeleteCompare(int id);
=======

        
>>>>>>> 0d58d4c1f5ca98968aae8a34be76e227ed1379b1
    }
}
