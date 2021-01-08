using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.WebRuleLib
{
    public partial class ASHC_WebRule
    {
        /*
         {
  "DanTiaoProbability": 1,
  "LotteryGameState": 0,
  "MaxPrizePerLongHuDanTiao": 0,
  "MaxPrizePerDanTiao": 30000,
  "MaxPrizePerIssue": 300000,
  "LotteryCategoryId": 2,
  "LotteryCategoryOffset": 0,
  "UserReturnPoint": 7.5,
  "LotteryGameId": 10,
  "RecentIssues": [
    {
      "Checked": false,
      "Multiple": 0,
      "BetAmount": 0,
      "EndTime": "2020/12/03 20:29:00",
      "GameSerialNumber": "20201203034"
    },
    {
      "Checked": false,
      "Multiple": 0,
      "BetAmount": 0,
      "EndTime": "2020/12/03 20:49:00",
      "GameSerialNumber": "20201203035"
    }  。。。
  ],
  "OldIssues": [
    {
      "EndTime": "2020/12/03 20:09:00",
      "OriginalWinningNumber": null,
      "GameWinningNumber": "01,07,11,03,08",
      "GameSerialNumber": "20201203033"
    },
    {
      "EndTime": "2020/12/03 19:49:00",
      "OriginalWinningNumber": null,
      "GameWinningNumber": "09,11,02,01,05",
      "GameSerialNumber": "20201203032"
    }。。。
  ],
  "MiaoMiaoWinningNumbers": null,
  "BetTypeInfos": {
    "1001": {
      "State": true,
      "AwardCount": 1,
      "TopAwardCode": 1001,
      "BetTypeCode": 1001,
      "Awards": {
        "1001": {
          "FixedPrize": 0,
          "PrizeDiscount": 2
        }
      }
    },
    "1002": {
      "State": true,
      "AwardCount": 1,
      "TopAwardCode": 1001,
      "BetTypeCode": 1002,
      "Awards": {
        "1001": {
          "FixedPrize": 0,
          "PrizeDiscount": 2
        }
      }
    }。。。
  },
  "OddsRestrict": null,
  "WinningNumberCombos": null,
  "WinningNumberStatistics": {
    "Miss": {
      "Position": [ 0, 15, 9, 5, 3, 4, 8, 13, 1, 12, 21, 5, 11, 25, 16, 22, 2, 0, 8, 24, 7, 1, 29, 1, 8, 9, 11, 5, 12, 6, 3, 26, 0, 1, 22, 0, 12, 5, 3, 4, 26, 58, 10, 2, 2, 8, 5, 3, 1, 14, 7, 0, 12, 15, 22 ],
      "OddEven": [ 1, 0, 0, 4, 0, 5, 0, 2, 0, 1, 0, 1, 2, 0, 0, 3, 0, 1, 1, 0 ],
      "Sum": null,
      "TripleSame": null,
      "TripleOrder": null,
      "DoubleSame": null,
      "DoubleSameSingleDiff": null,
      "Diff": null,
      "SumF3": null,
      "Longhu": null,
      "Triple": null,
      "Combination3": null,
      "Combination6": null,
      "MaxDifference": null,
      "NumberDistribution": null,
      "SumOddEven": null,
      "SumCombineOddEven": null,
      "SumBigSmallHe": null,
      "S5Combine": null,
      "S4Combine": null,
      "S3Arbitrary1": null,
      "S4Arbitrary1": null,
      "S5Arbitrary1": null,
      "SF2OddEven": null,
      "SL2OddEven": null,
      "SF3OddEven": null,
      "SL3OddEven": null,
      "PK10Longhu": null,
      "NiuNiu": null,
      "NiuNiuStud": null,
      "DoubleSameOverAll": null,
      "DiffOverAll": null,
      "S3Special": null,
      "GuessMatchNumber": null,
      "PK10OddEvenSumForPos1Pos2": null,
      "PK10SumForPos1Pos2Match": null,
      "SumLast": null,
      "Happy8UpDown": null,
      "Happy8OddEvenCount": null,
      "PrimeComposite": null,
      "NiuNiuTwoSide": null,
      "LastBigSmail": null,
      "CombinedOddEven": null
    },
    "Hot": {
      "Position": [ 13, 16, 11, 12, 9, 8, 9, 11, 15, 6, 10, 13, 11, 7, 13, 4, 13, 12, 12, 8, 14, 13, 6, 13, 14, 11, 14, 5, 15, 10, 16, 9, 7, 9, 10, 8, 9, 13, 16, 7, 11, 8, 13, 16, 9, 11, 13, 19, 8, 17, 7, 12, 8, 8, 8 ],
      "OddEven": [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
      "Sum": null,
      "TripleSame": null,
      "TripleOrder": null,
      "DoubleSame": null,
      "DoubleSameSingleDiff": null,
      "Diff": null,
      "SumF3": null,
      "Longhu": null,
      "Triple": null,
      "Combination3": null,
      "Combination6": null,
      "MaxDifference": null,
      "NumberDistribution": null,
      "SumOddEven": null,
      "SumCombineOddEven": null,
      "SumBigSmallHe": null,
      "S5Combine": null,
      "S4Combine": null,
      "S3Arbitrary1": null,
      "S4Arbitrary1": null,
      "S5Arbitrary1": null,
      "SF2OddEven": null,
      "SL2OddEven": null,
      "SF3OddEven": null,
      "SL3OddEven": null,
      "PK10Longhu": null,
      "NiuNiu": null,
      "NiuNiuStud": null,
      "DoubleSameOverAll": null,
      "DiffOverAll": null,
      "S3Special": null,
      "GuessMatchNumber": null,
      "PK10OddEvenSumForPos1Pos2": null,
      "PK10SumForPos1Pos2Match": null,
      "SumLast": null,
      "Happy8UpDown": null,
      "Happy8OddEvenCount": null,
      "PrimeComposite": null,
      "NiuNiuTwoSide": null,
      "LastBigSmail": null,
      "CombinedOddEven": null
    }
  },
  "LotteryOfficialURL": "http://www.gdlottery.cn/html/gplottery/"
}
             */
        public class ASHC_GameInfoReturnClass :JsonableClass<ASHC_GameInfoReturnClass>
        {
            public int LotteryCategoryId;
            public List<ASHC_PassedIssuesClass> OldIssues;
        }

        public class ASHC_PassedIssuesClass
        {
            /*"EndTime": "2020/12/03 20:09:00",
      "OriginalWinningNumber": null,
      "GameWinningNumber": "01,07,11,03,08",
      "GameSerialNumber": "20201203033"*/
            public string EndTime;//": "2020/12/03 20:09:00",
            public string OriginalWinningNumber;//": null,
            public string GameWinningNumber;//: "01,07,11,03,08",
            public string GameSerialNumber;
        }
    }
}
