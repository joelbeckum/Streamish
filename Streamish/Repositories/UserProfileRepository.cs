using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    SELECT Id, [Name], Email, ImageUrl, DateCreated
                    FROM UserProfile";

            using var reader = cmd.ExecuteReader();

            List<UserProfile> userProfiles = new List<UserProfile>();
            while (reader.Read())
            {
                userProfiles.Add(new UserProfile()
                {
                    Id = DbUtils.GetInt(reader, "Id"),
                    Name = DbUtils.GetString(reader, "Name"),
                    Email = DbUtils.GetString(reader, "Email"),
                    ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                    Videos = new List<Video>()
                });
            }
            return userProfiles;
        }

        public UserProfile GetById(int id)
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    SELECT Id, [Name], Email, ImageUrl, DateCreated
                    FROM UserProfile
                    WHERE Id = @id";

            DbUtils.AddParameter(cmd, "@id", id);

            using var reader = cmd.ExecuteReader();

            UserProfile userProfile = null;

            if (reader.Read())
            {
                userProfile = new UserProfile()
                {
                    Id = DbUtils.GetInt(reader, "Id"),
                    Name = DbUtils.GetString(reader, "Name"),
                    Email = DbUtils.GetString(reader, "Email"),
                    ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                    Videos = new List<Video>()
                };
            }
            return userProfile;
        }

        public UserProfile GetByIdWithVideos(int id)
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    SELECT up.Id, up.[Name], up.Email, up.ImageUrl, up.DateCreated,
                           v.Id AS videoId,
                           v.Title, v.Description, v.Url,
                           v.DateCreated AS videoDateCreated,
                           v.UserProfileId
                      FROM UserProfile up
                      LEFT JOIN Video v ON up.Id = v.UserProfileId
                     WHERE up.Id = @id";

            DbUtils.AddParameter(cmd, "@id", id);

            using var reader = cmd.ExecuteReader();

            UserProfile userProfile = null;

            while (reader.Read())
            {
                if (userProfile == null)
                {
                    userProfile = new UserProfile()
                    {
                        Id = DbUtils.GetInt(reader, "Id"),
                        Name = DbUtils.GetString(reader, "Name"),
                        Email = DbUtils.GetString(reader, "Email"),
                        ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                        Videos = new List<Video>()
                    };
                }
                if (DbUtils.IsNotDbNull(reader, "videoId"))
                {
                    userProfile.Videos.Add(new Video()
                    {
                        Id = DbUtils.GetInt(reader, "videoId"),
                        Title = DbUtils.GetString(reader, "Title"),
                        Description = DbUtils.GetString(reader, "Description"),
                        DateCreated = DbUtils.GetDateTime(reader, "videoDateCreated"),
                        Url = DbUtils.GetString(reader, "Url"),
                        UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                    });
                }
            }
            return userProfile;
        }

        public void Add(UserProfile userProfile)
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    INSERT INTO UserProfile ([Name], Email, ImageUrl, DateCreated)
                    OUTPUT INSERTED.Id
                    VALUES (@name, @email, @imageUrl, @dateCreated)";

            DbUtils.AddParameter(cmd, "@name", userProfile.Name);
            DbUtils.AddParameter(cmd, "@email", userProfile.Email);
            DbUtils.AddParameter(cmd, "@imageUrl", userProfile.ImageUrl);
            DbUtils.AddParameter(cmd, "@dateCreated", userProfile.DateCreated);

            userProfile.Id = (int)cmd.ExecuteScalar();
        }

        public void Update(UserProfile userProfile)
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    UPDATE UserProfile
                       SET [Name] = @name,
                           Email = @email,
                           ImageUrl = @imageUrl,
                           DateCreated = @dateCreated
                     WHERE Id = @id";

            DbUtils.AddParameter(cmd, "@id", userProfile.Id);
            DbUtils.AddParameter(cmd, "@name", userProfile.Name);
            DbUtils.AddParameter(cmd, "@email", userProfile.Email);
            DbUtils.AddParameter(cmd, "@imageUrl", userProfile.ImageUrl);
            DbUtils.AddParameter(cmd, "@dateCreated", userProfile.DateCreated);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Connection;
            conn.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"DELETE FROM UserProfile WHERE Id = @Id";

            DbUtils.AddParameter(cmd, "@id", id);

            cmd.ExecuteNonQuery();
        }
    }
}
