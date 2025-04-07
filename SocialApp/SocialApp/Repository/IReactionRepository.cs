using SocialApp.Entities;
using SocialApp.Enums;
using System.Collections.Generic;

namespace SocialApp.Repository
{
    public interface IReactionRepository
    {
        void DeleteByUserAndPost(long userId, long postId);
        List<Reaction> GetAll();
        List<Reaction> GetByPost(long postId);
        Reaction GetByUserAndPost(long userId, long postId);
        void Save(Reaction entity);
        void UpdateByUserAndPost(long userId, long postId, ReactionType type);
    }
}