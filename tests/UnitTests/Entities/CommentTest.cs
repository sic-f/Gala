using Core.Dto;
using Core.Entities.Comment;

namespace UnitTests.Entities;

public class CommentTest
{
    public class SetDetailsTest
    {
        [Theory]
        [InlineData("Comment Body")]
        [InlineData("Different Comment Body")]
        public void BodyTest(string body)
        {
            CommentDetails details = new() { Body = body };
            PlacePictureComment comment = new();

            comment.SetDetails(details, Guid.Empty, Guid.Empty);

            Assert.Equal(body, comment.Body);
        }

        [Fact]
        public void CommentableAndCommenterIdTest()
        {
            var placePictureId = Guid.NewGuid();
            var broId = Guid.NewGuid();

            PlacePictureComment comment = new();
            CommentDetails details = new();
            comment.SetDetails(details, placePictureId, broId);

            Assert.Equal(placePictureId, comment.CommentableId);
            Assert.Equal(broId, comment.CommenterId);
        }
    }
}