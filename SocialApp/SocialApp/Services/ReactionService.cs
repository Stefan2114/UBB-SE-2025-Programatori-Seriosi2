namespace SocialApp.Services
{
    using System;
    using System.Collections.Generic;
    using SocialApp.Entities;
    using SocialApp.Enums;
    using SocialApp.Repository;

    public class ReactionService(IReactionRepository reactionRepository) : IReactionService
    {
        public Reaction ValidateAdd(long userId, long postId, ReactionType type)
        {
            if (reactionRepository.GetByUserAndPost(userId, postId) != null)
            {
                reactionRepository.UpdateByUserAndPost(userId, postId, type);
                return reactionRepository.GetByUserAndPost(userId, postId);
            }

            Reaction reaction = new Reaction() { UserId = userId, PostId = postId, Type = type };
            reactionRepository.Save(reaction);
            return reaction;
        }

        public void ValidateDelete(long userId, long postId)
        {
            Reaction reaction = reactionRepository.GetByUserAndPost(userId, postId);
            if (reaction == null)
            {
                throw new Exception("Reaction does not exist");
            }

            reactionRepository.DeleteByUserAndPost(userId, postId);
        }

        public List<Reaction> GetAll()
        {
            return reactionRepository.GetAll();
        }

        public List<Reaction> GetReactionsForPost(long postId)
        {
            return reactionRepository.GetByPost(postId);
        }
    }
}
