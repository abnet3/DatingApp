using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int SourceUserId, int LikedUserId)
        {
            return await _context.Likes.FindAsync(SourceUserId, LikedUserId);
        }

        // public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        //a fter like list pagination
        // public async Task<PagedList<LikeDto>> GetUserLikes(string predicate, int userId)
                public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)


        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            // if (predicate == "liked")
            if (likesParams.predicate == "liked")

            {
                // likes = likes.Where(like => like.SourceUserId == userId);
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);

                users = likes.Select(like => like.LikedUser);
            }

           // if (predicate == "likedBy")
            if (likesParams.predicate == "likedBy")
            {
                // likes = likes.Where(like => like.LikedUserId == userId);
                  likes = likes.Where(like => like.LikedUserId == likesParams.UserId);

                users = likes.Select(like => like.SourceUser);

            }

            // return await users.Select(user => new LikeDto
        var likedUsers =  users.Select(user => new LikeDto

            {

                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateofBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            //  }).ToListAsync();
            });
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }


        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}