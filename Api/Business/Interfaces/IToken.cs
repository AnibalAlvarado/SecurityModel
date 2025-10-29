using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IToken
    {
        //Task<string> GenerateToken(LoginUserDto dto);
        /// <summary>
        /// Valida credenciales y emite Access + Refresh + CSRF.
        /// </summary>
        Task<(string AccessToken, string RefreshToken, string CsrfToken)> GenerateTokensAsync(LoginRequestDto dto);

        /// <summary>
        /// Rota el refresh token y devuelve nuevo Access + Refresh.
        /// </summary>
        Task<(string NewAccessToken, string NewRefreshToken)> RefreshAsync(string refreshTokenPlain, string remoteIp = null);

        /// <summary>
        /// Revoca explícitamente un refresh token.
        /// </summary>
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
