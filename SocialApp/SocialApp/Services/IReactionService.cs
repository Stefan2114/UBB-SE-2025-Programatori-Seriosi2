using SocialApp.Entities;
using SocialApp.Enums;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IReactionService
    {
        List<Reaction> GetAll();

        List<Reaction> GetReactionsForPost(long postId);

        Reaction ValidateAdd(long userId, long postId, ReactionType type);

        void ValidateDelete(long userId, long postId);
    }
}