using SocialApp.Entities;
using SocialApp.Enums;
using System.Collections.Generic;

namespace SocialApp.Services
{
    public interface IReactionService
    {
        List<Reaction> GetAllReactions();

        List<Reaction> GetReactionsForPost(long postId);

        Reaction AddReaction_byUserAndPost(long userId, long postId, ReactionType type);

        void DeleteReaction_byUserAndPost(long userId, long postId);
    }
}