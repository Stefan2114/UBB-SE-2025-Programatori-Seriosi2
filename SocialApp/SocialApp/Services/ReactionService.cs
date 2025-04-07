﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApp.Entities;
using SocialApp.Enums;
using SocialApp.Repository;

namespace SocialApp.Services
{
    public class ReactionService : IReactionService
    {
        private IReactionRepository ReactionRepository;

        public ReactionService(IReactionRepository reactionRepository)
        {
            this.ReactionRepository = reactionRepository;
        }

        public Reaction ValidateAdd(long userId, long postId, ReactionType type)
        {
            if (ReactionRepository.GetByUserAndPost(userId, postId) != null)
            {
                ReactionRepository.UpdateByUserAndPost(userId, postId, type);
                return ReactionRepository.GetByUserAndPost(userId, postId);
            }
            Reaction reaction = new Reaction() { UserId = userId, PostId = postId, Type = type };
            ReactionRepository.Save(reaction);
            return reaction;
        }

        public void ValidateDelete(long userId, long postId)
        {
            Reaction reaction = ReactionRepository.GetByUserAndPost(userId, postId);
            if (reaction == null)
            {
                throw new Exception("Reaction does not exist");
            }
            ReactionRepository.DeleteByUserAndPost(userId, postId);
        }
        public List<Reaction> GetAll()
        {
            return ReactionRepository.GetAll();
        }

        public List<Reaction> GetReactionsForPost(long postId)
        {
            return ReactionRepository.GetByPost(postId);
        }
    }
}
