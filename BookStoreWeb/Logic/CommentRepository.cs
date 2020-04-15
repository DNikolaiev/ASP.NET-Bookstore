using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using BookStoreWeb.Models;
namespace BookStoreWeb.Logic
{
    public class CommentRepository
    {
        public Comment GetComment(int id)
        {
            using (var db = new BookStoreContext())
            {
                return db.Comments.Where(x => x.Id == id).ToList().FirstOrDefault();
            }
        }
        public bool IsCommentUpvoted(int id, string username)
        {
            if (username == null) return false;
            using (var db = new BookStoreContext())
            {
                int count = db.UserRatedComments.Count(x => x.CommentID == id && x.UserName == username &&x.Status==CommentStatus.Upvoted.ToString());
                if (count > 0) return true;
                else return false;
            }
                
        }
        public bool IsCommentDownvoted(int id, string username)
        {
            if (username == null) return false ;
            using (var db = new BookStoreContext())
            {
                int count = db.UserRatedComments.Count(x => x.CommentID == id && x.UserName == username && x.Status == CommentStatus.Downvoted.ToString());
                if (count > 0) return true;
                else return false;
            }
        }
        public Tuple<int,int> UpvoteComment(int id, string username)
        {
            using (var db = new BookStoreContext())
            {
                Comment comment = GetComment(id);
                if (comment == null || username==null) return Tuple.Create(0,0);
                if (!IsCommentUpvoted(id, username) && !IsCommentDownvoted(id,username)) //not rated
                {
                    // just upvote
                    comment.UpVotes++;
                    db.UserRatedComments.Add(new UserRatedComment
                    {
                        CommentID = id,
                        UserName = username,
                        Status = CommentStatus.Upvoted.ToString()
                    });

                }
                else if (IsCommentDownvoted(id, username))
                {
                    comment.DownVotes--;
                    comment.UpVotes++;
                    var matches = db.UserRatedComments.Where(
                        x => x.UserName == username && x.CommentID == id).FirstOrDefault();
                    if (matches == null) return Tuple.Create(0,0);
                    var item = db.UserRatedComments.Find(matches.Id);
                    if (item == null) return Tuple.Create(0,0);
                    item.Status = CommentStatus.Upvoted.ToString();
                    db.Entry(item).CurrentValues.SetValues(item);
                }
                else if (IsCommentUpvoted(id,username))
                {
                    comment.UpVotes--;
                    var matches = db.UserRatedComments.Where(
                        x => x.UserName == username && x.CommentID == id).FirstOrDefault();
                    if (matches == null) return Tuple.Create(0, 0);
                    var item = db.UserRatedComments.Find(matches.Id);
                    if (item == null) return Tuple.Create(0, 0);
                    db.UserRatedComments.Remove(item);
                }

                
                    var entity = db.Comments.Find(id);
                    if (entity == null)
                    {
                        return Tuple.Create(0, 0);
                    }
                    db.Entry(entity).CurrentValues.SetValues(comment);
                    db.SaveChanges();
                return Tuple.Create(comment.UpVotes, comment.DownVotes);
            }
        }
        public Tuple<int,int> DownvoteComment(int id, string username)
        {
            using (var db = new BookStoreContext())
            {

                Comment comment = GetComment(id);
                if (comment == null || username == null) return Tuple.Create(0, 0);
              
                if (!IsCommentUpvoted(id, username) && !IsCommentDownvoted(id, username)) //not rated
                {
                    // just downvote
                    comment.DownVotes++;

                    db.UserRatedComments.Add(new UserRatedComment
                    {
                        CommentID = id,
                        UserName = username,
                        Status = CommentStatus.Downvoted.ToString()
                    });
                }
                else if (IsCommentUpvoted(id, username))
                {
                    comment.DownVotes++;
                    comment.UpVotes--;

                    var matches = db.UserRatedComments.Where(
                        x => x.UserName == username && x.CommentID == id).FirstOrDefault();
                    if (matches == null) return Tuple.Create(0, 0);
                    var item = db.UserRatedComments.Find(matches.Id);
                    if (item == null) return Tuple.Create(0, 0);
                    item.Status = CommentStatus.Downvoted.ToString();
                    db.Entry(item).CurrentValues.SetValues(item);
                }
                else if (IsCommentDownvoted(id, username))
                {
                    comment.DownVotes--;

                    var matches = db.UserRatedComments.Where(
                        x => x.UserName == username && x.CommentID == id).FirstOrDefault();
                    if (matches == null) return Tuple.Create(0, 0);
                    var item = db.UserRatedComments.Find(matches.Id);
                    if (item == null) return Tuple.Create(0, 0);
                    db.UserRatedComments.Remove(item);
                }
            
                    var entity = db.Comments.Find(id);
                    if (entity == null)
                    {
                        return Tuple.Create(0, 0);
                }
                    db.Entry(entity).CurrentValues.SetValues(comment);
                    db.SaveChanges();
                return Tuple.Create(comment.UpVotes,comment.DownVotes);
            }
        }
        public void AddComment(Comment comment)
        {
            using (var db = new BookStoreContext())
            {
                db.Comments.Add(comment);
                db.SaveChanges();
            }
        }
        public void DeleteComment(int comment_id)
        {
            using (var db = new BookStoreContext())
            {
                var comment_to_delete = db.Comments.Find(comment_id);
                if (comment_to_delete == null) return;
                var ratedComments =
                    from c in db.UserRatedComments
                    where c.CommentID == comment_id
                    select new { Comment = c };
                foreach(var item in ratedComments)
                {
                    if(item!=null)
                    db.UserRatedComments.Remove(item.Comment);
                }

                db.Comments.Remove(comment_to_delete);
                db.SaveChanges();
            }
        }
        
    }
}