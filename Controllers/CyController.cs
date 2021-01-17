using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Flurl;
using Flurl.Http;

namespace CyphersAddon.Controllers
{
    [ApiController]
    [Route("cy")]
    public class CyController : ControllerBase
    {
        //아이템 이미지 URL : https://img-api.neople.co.kr/cy/items/<itemId>
        //캐릭터 이미지 URL : https://img-api.neople.co.kr/cy/characters/<characterId>
        //포지션 특성 이미지 URL : https://img-api.neople.co.kr/cy/position-attributes/<attributeId>

        private readonly Url baseUrl =
            "https://api.neople.co.kr/cy"
            .SetQueryParam("apikey", Environment.GetEnvironmentVariable("CY_APIKEY")!);

        private readonly ILogger<CyController> _logger;

        public CyController(ILogger<CyController> logger)
        {
            _logger = logger;
        }

        [HttpGet("players")]
        public async Task<dynamic> players
        (
            [FromQuery] string nickname,
            [FromQuery][RegularExpression("match|full")][DefaultValue("match")] string wordType,
            [FromQuery][Range(1, 200)][DefaultValue(10)] int limit
        )
        {
            return await baseUrl
                .AppendPathSegment("players")
                .SetQueryParams(new { nickname, wordType, limit })
                .GetJsonAsync();
        }
        [HttpGet("players/{playerId}")]
        public async Task<dynamic> player([FromRoute] string playerId)
        {
            return await baseUrl
                .AppendPathSegment($"players/{playerId}")
                .GetJsonAsync();
        }
        [HttpGet("players/{playerId}/matches")]
        public async Task<dynamic> playerMatches
        (
            [FromRoute] string playerId,
            [FromQuery][RegularExpression("rating|normal")][DefaultValue("rating")] string gameTypeId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery][Range(1, 100)][DefaultValue(10)] int limit,
            [FromQuery] string? next
        //startDate, endDate 요청 변수 사용 예시
        //ex) /matches? startDate = 20180901T0000&endDate=20180930T2359
        ///matches? startDate = 2018 - 09 - 01 00:00&endDate=2018-09-30 23:59
        //※ 기간 검색 요청 시 하나의 파라미터(startDate, endDate)라도 누락 될 경우 검색이 불가능합니다.
        //※ 다양한 Date Type을 지원하지만, 상황에 따라 변경될 수 있으므로 예시 Date Type을 이용 부탁드립니다.
        //※ 초 단위 설정의 경우 startDate : 00초, endDate : 59초로 처리됩니다.
        //※ 기간 설정은 최대 90일까지 가능합니다.
        //next 요청변수 사용 예시
        //ex) /matches? next =< next > &apikey =< apikey >
        //※ next 사용 시 gameTypeId, limit, startDate, endDate등 요청변수는 최초 조회 기준으로 적용됩니다.
        )
        {
            return await baseUrl
                .AppendPathSegment($"players/{playerId}/matches")
                .SetQueryParams(new { gameTypeId, startDate, endDate, limit, next })
                .GetJsonAsync();
        }
        [HttpGet("matches/{matchId}")]
        public async Task<dynamic> match([FromRoute] string matchId)
        {
            return await baseUrl
                .AppendPathSegment($"matches/{matchId}")
                .GetJsonAsync();
        }
        [HttpGet("ranking/ratingpoint")]
        public async Task<dynamic> playerRanking
        (
            [FromQuery] string playerId,
            [FromQuery][DefaultValue(0)] int offset,
            [FromQuery][Range(1, 1000)][DefaultValue(10)] int limit
        )
        {
            return await baseUrl
                .AppendPathSegment("ranking/ratingpoint")
                .SetQueryParams(new { playerId, offset, limit })
                .GetJsonAsync();
        }
        [HttpGet("ranking/characters/{characterId}/{rankingType}")]
        public async Task<dynamic> characterRanking
        (
            [FromRoute] string characterId,
            [FromRoute][RegularExpression("winCount|winRate|killCount|assistCount|exp")] string rankingType,
            [FromQuery] string playerId,
            [FromQuery][DefaultValue(0)] int offset,
            [FromQuery][Range(1, 1000)][DefaultValue(10)] int limit
        )
        {
            return await baseUrl
                .AppendPathSegment($"ranking/characters/{characterId}/{rankingType}")
                .SetQueryParams(new { playerId, offset, limit })
                .GetJsonAsync();
        }
        [HttpGet("ranking/tsj/{tsjType}")]
        public async Task<dynamic> tsjRanking
        (
            [FromRoute][RegularExpression("melee|ranged")] string tsjType,
            [FromQuery] string playerId,
            [FromQuery][DefaultValue(0)] int offset,
            [FromQuery][Range(1, 1000)][DefaultValue(10)] int limit
        )
        {
            return await baseUrl
                .AppendPathSegment($"ranking/tsj/{tsjType}")
                .SetQueryParams(new { playerId, offset, limit })
                .GetJsonAsync();
        }
        [HttpGet("battleitems")]
        public async Task<dynamic> battleitems
        (
            [FromQuery][Range(1, 1000)][DefaultValue(10)] int limit,
            [FromQuery] string itemName,
            [FromQuery][RegularExpression("match|front|full")][DefaultValue("match")] string wordType,
            [FromQuery] string[]? q
        )
        {
            return await baseUrl
                .AppendPathSegment("battleitems")
                .SetQueryParams(new { limit, itemName, wordType, q })
                .GetJsonAsync();
        }
        [HttpGet("battleitems/{itemId}")]
        public async Task<dynamic> battleitem
        (
            [FromRoute] string itemId
        )
        {
            return await baseUrl
                .AppendPathSegment($"battleitems/{itemId}")
                .GetJsonAsync();
        }
        [HttpGet("multi/battleitems")]
        public async Task<dynamic> multiBattleitem
        (
            // 아이템 고유 코드 쉼표(,) 를 통한 구분 값 처리
            // 아이템 최대 30개 조회
            [FromQuery] string itemIds
        )
        {
            return await baseUrl
                .AppendPathSegment("multi/battleitems")
                .SetQueryParams(new { itemIds })
                .GetJsonAsync();
        }
        [HttpGet("characters")]
        public async Task<dynamic> characters()
        {
            return await baseUrl
                .AppendPathSegment("characters")
                .GetJsonAsync();
        }
        [HttpGet("position-attributes/{attributeId}")]
        public async Task<dynamic> positionAttributes
        (
            [FromRoute] string attributeId
        )
        {
            return await baseUrl
                .AppendPathSegment($"position-attributes/{attributeId}")
                .GetJsonAsync();
        }
    }
}
