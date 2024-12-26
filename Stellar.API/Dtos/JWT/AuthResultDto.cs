namespace Stellar.API.JWT.Dtos
{
    public record AuthResultDto
    {
        public string Token { get; init; } // 使用 init 訪問器使其可設置但不可變
        public long ExpireTime { get; init; }
    }
}
