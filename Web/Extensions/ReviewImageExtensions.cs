using Web.Enums;

namespace Web.Extensions
{
    public static class ReviewImageExtensions
    {
        public static string GetImageUrl(this ReviewImage reviewImage)
        {
            return reviewImage switch
            {
                ReviewImage.Good => "https://store.akamai.steamstatic.com/public/images/v6/user_reviews_positive.png",
                ReviewImage.NotGood => "https://store.akamai.steamstatic.com/public/images/v6/user_reviews_negative.png",
                ReviewImage.Mixed => "https://store.akamai.steamstatic.com/public/images/v6/user_reviews_mixed.png",
                _ => string.Empty
            };
        }

        public static string GetReviewImageUrl(this int commentCount)
        {
            return commentCount switch
            {
                0 => ReviewImage.Nothing.GetImageUrl(), // 沒有評論
                > 5 => ReviewImage.Good.GetImageUrl(), // 讚
                <= 4 => ReviewImage.NotGood.GetImageUrl(), // 不好
                _ => ReviewImage.Mixed.GetImageUrl() // 混合
            };
        }
    }

}
