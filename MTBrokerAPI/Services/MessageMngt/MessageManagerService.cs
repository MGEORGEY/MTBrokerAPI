using Microsoft.EntityFrameworkCore;
using MTBrokerAPI.ControllerReturnTypes;
using MTBrokerAPI.ControllerReturnTypes.MessageMngt;
using MTBrokerAPI.Model;
using MTBrokerAPI.ModelViewModels.FileMngt;
using MTBrokerAPI.Services.Dossier;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MTBrokerAPI.Services.MessageMngt
{
    public class MessageManagerService : IMessageManager
    {

        #region Get Stored MT940s
        public async IAsyncEnumerable<MT940CRT> GetStoredMT940sAsync(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var mt940s = await applicationDbContext.MT940s.ToListAsync(cancellationToken);

            List<MT940CRT> mT940CRTs = new();

            for (int mt940Index = 0; mt940Index < mt940s.Count; mt940Index++)
            {
                yield return new MT940CRT
                {
                    MT940MonoCRT = new MT940MonoCRT
                    {
                        ID = mt940s[mt940Index].ID,
                        AccountID25 = mt940s[mt940Index].AccountID25,
                        FinSwiftAddress = mt940s[mt940Index].FinSwiftAddress,
                        FinBranchCode = mt940s[mt940Index].FinBranchCode,
                        FinLTCode = mt940s[mt940Index].FinLTCode,
                        OpeningBalance60FAmount = mt940s[mt940Index].AvailableBalance64Amount,
                        AvailableBalance64Amount = mt940s[mt940Index].AvailableBalance64Amount,
                        OpeningBalance60FCurrency = mt940s[mt940Index].OpeningBalance60FCurrency,
                        OpeningBalance60FDate = mt940s[mt940Index].OpeningBalance60FDate,
                        ClosingBalance62FAmount = mt940s[mt940Index].ClosingBalance62FAmount,
                        AvailableBalance64Date = mt940s[mt940Index].AvailableBalance64Date,
                        OpeningBalance60FDOrC = mt940s[mt940Index].OpeningBalance60FDOrC,
                        ClosingBalance62FCurrency = mt940s[mt940Index].ClosingBalance62FCurrency,
                        ClosingBalance62FDate = mt940s[mt940Index].ClosingBalance62FDate,
                        ClosingBalance62FDOrC = mt940s[mt940Index].ClosingBalance62FDOrC,
                        AvailableBalance64Currency = mt940s[mt940Index].AvailableBalance64Currency,
                        AvailableBalance64DOrC = mt940s[mt940Index].AvailableBalance64DOrC,
                        StatementOrSeqNo28CMsgSeq = mt940s[mt940Index].StatementOrSeqNo28CMsgSeq,
                        SendersSwiftAddress = mt940s[mt940Index].SendersSwiftAddress,
                        SequenceNumber = mt940s[mt940Index].SequenceNumber,
                        SessionNumber = mt940s[mt940Index].SessionNumber,
                        StatementOrSeqNo28CStmntSeq = mt940s[mt940Index].StatementOrSeqNo28CStmntSeq,
                        TransactionRefNo20 = mt940s[mt940Index].TransactionRefNo20

                    },
                    Tag61And86GroupCRTs = await applicationDbContext.Tag61And86Groups.Where(n => n.MT940 == mt940s[mt940Index]).Select(n => new Tag61And86GroupCRT
                    {
                        ID = n.ID,
                        MT940ID = n.MT940ID,
                        AccOwnerInfo86Info = n.AccOwnerInfo86Info,
                        StatementLine61Amount = n.StatementLine61Amount,
                        StatementLine61BankRef = n.StatementLine61BankRef,
                        StatementLine61CustomerRef = n.StatementLine61CustomerRef,
                        StatementLine61DOrC = n.StatementLine61DOrC,
                        StatementLine61EntryDate = n.StatementLine61EntryDate,
                        StatementLine61FundsCode = n.StatementLine61FundsCode,
                        StatementLine61Suppliment = n.StatementLine61Suppliment,
                        StatementLine61TrnsactnTypeID = n.StatementLine61TrnsactnTypeID,
                        StatementLine61ValueDate = n.StatementLine61ValueDate
                    }).ToListAsync()
                };
            }
        }
        #endregion


        #region Parse MT940
        public async Task<ParseMT940WithStatusCRT> ParseMT940(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, string filePath)
        {
            ParseMT940WithStatusCRT parseMT940WithStatusCRT = new() { MT940CRTs = new() { }, SuccessStatusMessageCRT = new() };

            MT940 mt940 = null;

            List<Tag61And86Group> tag61And86Groups = null;

            try
            {
                var message = File.ReadAllText(filePath);

                if (string.IsNullOrEmpty(message)) return new ParseMT940WithStatusCRT
                {
                    MT940CRTs = new(),
                    SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.UnreadableFileErrorMessage, Success = false }
                };

                #region Block 1 (MANDATORY)

                var basicHeaderID = "(?<basicHeaderID>[{][1][:])";

                var applicationID = "(?<applicationID>[F])";

                var serviceID = "(?<serviceID>[0][1])";

                var finSwiftAddress = @"(?<finSwiftAddress>[A-Za-z0-9/–?:().,‘+\s]{8})";

                var finLTCode = @"(?<finLTCode>[A-Za-z0-9/–?:().,‘+\s]{1})";

                var finBranchCode = @"(?<finBranchCode>[A-Za-z0-9/–?:().,‘+\s]{3})";

                var sessionNumber = @"(?<sessionNumber>[A-Za-z0-9/–?:().,‘+\s]{4})";

                var sequenceNumber = @"(?<sequenceNumber>[A-Za-z0-9/–?:().,‘+\s]{6})";

                var block1Tail = @"(?<block1Tail>[}])";



                var block1Pattern = $"^{basicHeaderID}{applicationID}{serviceID}{finSwiftAddress}{finLTCode}{finBranchCode}{sessionNumber}{sequenceNumber}{block1Tail}";

                #endregion


                #region Block 2 (OPTIONAL)

                var applicationHeaderID = "(?<applicationHeaderID>([{][2][:]))";

                var inputOutputID = "(?<inputOutputID>[O|I])";

                var messageType = @"(?<messageType>[\d]{3})";

                var sendersSwiftAddress = @"(?<sendersSwiftAddress>[A-Za-z0-9/–?:().,‘+\s]{1,12})";

                var messagePriority = @"(?<messagePriority>[N])";

                var block2Tail = @"(?<block2Tail>[}])";


                var block2Pattern = $"({applicationHeaderID}{inputOutputID}{messageType}{sendersSwiftAddress }{messagePriority}{block2Tail})?";

                #endregion


                #region Block 4 (MANDATORY)

                var block4HeaderID = "(?<block4HeaderID>[{][4][:][\r\n]*)";

                var transactionRefNo20Header = "(?<transactionRefNo20Header>[:][2][0][:])";

                var transactionRefNo20 = "(?<transactionRefNo20>[A-Za-z0-9]{1,16})";

                var transactionRefNo20CrLf = "(?<transactionRefNo20CrLf>[\r\n]*)";

                //var relatedReference21Pattern = "(?<relatedReference21>[:][2][1][:]([A-Za-z0-9/–?:().,‘+\\s]{0,32})[\r\n]*)?";

                var accountID25Header = "(?<accountID25Header>[:][2][5][:])";

                var accountID25 = "(?<accountID25>[A-Za-z0-9]{0,35})";

                var accountID25CrLf = "(?<accountID25CrLf>[\r\n]*)";

                var statementOrSeqNo28CHeader = @"(?<statementOrSeqNo28CHeader>[:][2][8][C][:])";

                var statementOrSeqNo28CStmntSeq = @"(?<statementOrSeqNo28CStmntSeq>[\d]{1,5})";

                var statementOrSeqNo28COr = @"(?<statementOrSeqNo28COr>[/])?";

                var statementOrSeqNo28CMsgSeq = @"(?<statementOrSeqNo28CMsgSeq>[\d]{0,5})?";

                var statementOrSeqNo28CCrLf = "(?<statementOrSeqNo28CCrLf>[\r\n]*)";

                var openingBalance60FOrMHeader = @"(?<openingBalance60FOrMHeader>[:][6][0][F|M][:])";

                var openingBalance60FDOrC = @"(?<openingBalance60FDOrC>[D|C])";

                var openingBalance60FDate = @"(?<openingBalance60FDate>[\d]{6})";

                var openingBalance60FCurrency = @"(?<openingBalance60FCurrency>[A-Z]{3})";

                var openingBalance60FAmount = @"(?<openingBalance60FAmount>[\d]{1,13}[,][\d]{1,2}|[\d]{1,15})";

                var openingBalance60FCrLf = "(?<openingBalance60FCrLf>[\r\n]*)";

                var tag60FATWT62 = "(?<tag60FATWT62>.*?(?=:62))";

                var closingBalance62FOrMHeader = @"(?<closingBalance62FOrMHeader>[:][6][2][F|M][:])";

                var closingBalance62FDOrC = @"(?<closingBalance62FDOrC>[D|C])";

                var closingBalance62FDate = @"(?<closingBalance62FDate>[\d]{6})";

                var closingBalance62FCurrency = @"(?<closingBalance62FCurrency>[A-Z]{3})";

                var closingBalance62FAmount = @"(?<closingBalance62FAmount>[\d]{1,13}[,][\d]{1,2}|[\d]{1,15})";

                var closingBalance62FCrLf = "(?<closingBalance62FCrLf>[\r\n]*)";


                var availableBalance64Header = @"(?<availableBalance64Header>[:][6][4][:])";

                var availableBalance64DOrC = @"(?<availableBalance64DOrC>[D|C])";

                var availableBalance64Date = @"(?<availableBalance64Date>[\d]{6})";

                var availableBalance64Currency = @"(?<availableBalance64Currency>[A-Z]{3})";

                var availableBalance64Amount = @"(?<availableBalance64Amount>[\d]{1,13}[,][\d]{1,2}|[\d]{1,15})";

                var availableBalance64CrLf = "(?<availableBalance64CrLf>[\r\n]*)";

                var block4Closing = "(?<block4Closing>[-][}][\r\n]*)$";

                var block4Pattern = $"{block4HeaderID}{transactionRefNo20Header}{transactionRefNo20}{transactionRefNo20CrLf}{accountID25Header}{accountID25}{accountID25CrLf}{statementOrSeqNo28CHeader}{statementOrSeqNo28CStmntSeq}{statementOrSeqNo28COr}{statementOrSeqNo28CMsgSeq}{statementOrSeqNo28CCrLf}{openingBalance60FOrMHeader}{openingBalance60FDOrC}{openingBalance60FDate}{openingBalance60FCurrency}{openingBalance60FAmount}{openingBalance60FCrLf}";


                #endregion


                var match = Regex.Match(message, $"{block1Pattern}{block2Pattern}{block4Pattern}");

                if (match.Success)
                {

                    mt940 = new MT940
                    {
                        AccountID25 = match.Groups[nameof(accountID25)].Value ?? string.Empty,
                        FinBranchCode = match.Groups[nameof(finBranchCode)].Value ?? string.Empty,
                        FinLTCode = match.Groups[nameof(finLTCode)].Value ?? string.Empty,
                        FinSwiftAddress = match.Groups[nameof(finSwiftAddress)].Value ?? string.Empty,
                        OpeningBalance60FAmount = match.Groups[nameof(openingBalance60FAmount)].Value.ToMoney(),
                        OpeningBalance60FCurrency = match.Groups[nameof(openingBalance60FCurrency)].Value ?? string.Empty,
                        OpeningBalance60FDate = match.Groups[nameof(openingBalance60FDate)].Value.ToDate(),
                        OpeningBalance60FDOrC = match.Groups[nameof(openingBalance60FDOrC)].Value ?? string.Empty,
                        SendersSwiftAddress = match.Groups[nameof(sendersSwiftAddress)].Value ?? string.Empty,
                        SequenceNumber = match.Groups[nameof(sequenceNumber)].Value ?? string.Empty,
                        SessionNumber = match.Groups[nameof(sessionNumber)].Value ?? string.Empty,
                        StatementOrSeqNo28CMsgSeq = match.Groups[nameof(statementOrSeqNo28CMsgSeq)].Value ?? string.Empty,
                        StatementOrSeqNo28CStmntSeq = match.Groups[nameof(statementOrSeqNo28CStmntSeq)].Value ?? string.Empty,
                        TransactionRefNo20 = match.Groups[nameof(transactionRefNo20)].Value ?? string.Empty,
                    };

                    tag61And86Groups = new List<Tag61And86Group>();

                    var tags61And86 = message.Substring(message.IndexOf(":61:"));

                    if (!string.IsNullOrEmpty(tags61And86))
                    {
                        var tag61Indices = new List<int>();
                        var tag86Indices = new List<int>();

                        tags61And86.ReplaceLineEndings();

                        int index61 = 0, index86 = 0;
                        do
                        {
                            index61 = tags61And86.IndexOf(":61:", index61);
                            if (index61 != -1)
                            {
                                tag61Indices.Add(index61);
                                index61++;
                            }
                        } while (index61 != -1);

                        do
                        {
                            index86 = tags61And86.IndexOf(":86:", index86);
                            if (index86 != -1)
                            {
                                tag86Indices.Add(index86);
                                index86++;
                            }
                        } while (index86 != -1);


                        #region Regex for Tags 61 and 86
                        for (int tag61Index = 0, tag86Index = 0; tag61Index < tag61Indices.Count && tag86Index < tag86Indices.Count; tag61Index++, tag86Index++)
                        {

                            #region TAG 61
                            var tag61Text = tags61And86.Substring(tag61Indices[tag61Index]);


                            #region TAG 61 Rules

                            var statementLine61Header = @"(?<statementLine61Header>[:][6][1][:])";

                            var statementLine61ValueDate = @"(?<statementLine61ValueDate>[\d]{6})";

                            var statementLine61EntryDate = @"(?<statementLine61EntryDate>[\d]{4})?";

                            var statementLine61DOrC = @"(?<statementLine61DOrC>[D|C])";

                            var statementLine61FundsCode = @"(?<statementLine61FundsCode>[A-Z]{1})?";

                            var statementLine61Amount = @"(?<statementLine61Amount>[\d]{1,13}[,][\d]{1,2}|[\d]{1,15})";

                            var statementLine61TrnsactnTypeID = @"(?<statementLine61TrnsactnTypeID>[A-Z]{1}[A-Za-z0-9]{3})";

                            var statementLine61CustomerRef = "(?<statementLine61CustomerRef>[A-Za-z0-9]{0,16})";

                            var stmntLine61BankRefDoubleSlash = "(?<stmntLine61BankRefDoubleSlash>[/][/])";


                            var statementLine61BankRef = "(?<statementLine61BankRef>[A-Za-z0-9/–?:().,‘+\\s]{0,16}[\r\n]*)";

                            var statementLine61Suppliment = "(?<statementLine61Suppliment>(?:([A-Za-z0-9/-?:().,‘+][^\\S\r\n]*){0,35}))?";

                            var statementLine61FCrLf = "(?<statementLine61FCrLf>[\r\n]*)";



                            var tag61Format = $"{statementLine61Header}{statementLine61ValueDate}{statementLine61EntryDate}{statementLine61DOrC}{statementLine61FundsCode}{statementLine61Amount}{statementLine61TrnsactnTypeID}{statementLine61CustomerRef}({stmntLine61BankRefDoubleSlash}{statementLine61BankRef})?{statementLine61Suppliment}{statementLine61FCrLf}";

                            #endregion


                            var tag61Match = Regex.Match(tag61Text, tag61Format);

                            #endregion


                            #region TAG 86
                            var tag86Text = tags61And86.Substring(tag86Indices[tag86Index]);


                            #region TAG 86 Rules

                            var accOwnerInfo86Header = @"(?<accOwnerInfo86Header>[:][8][6][:])";


                            var accOwnerInfo86Info = "(?<accOwnerInfo86Info>([A-Za-z0-9/\\-?:().,‘+][^\\S\r\n]*){0,65})";


                            var accOwnerInfo86CrLf = "(?<accOwnerInfo86CrLf>[\r\n]*.*?(?=:6))";



                            var tag86Format = $"{accOwnerInfo86Header}{accOwnerInfo86Info}{accOwnerInfo86CrLf}";

                            #endregion


                            var tag86Match = Regex.Match(tag86Text, tag86Format);

                            if (tag61Match.Success && tag86Match.Success)
                            {
                                tag61And86Groups.Add(new Tag61And86Group
                                {
                                    AccOwnerInfo86Info = tag86Match.Groups[nameof(accOwnerInfo86Info)].Value ?? string.Empty,
                                    StatementLine61Amount = tag61Match.Groups[nameof(statementLine61Amount)].Value.ToMoney(),
                                    StatementLine61BankRef = tag61Match.Groups[nameof(statementLine61BankRef)].Value ?? string.Empty,
                                    StatementLine61CustomerRef = tag61Match.Groups[nameof(statementLine61CustomerRef)].Value ?? string.Empty,
                                    StatementLine61DOrC = tag61Match.Groups[nameof(statementLine61DOrC)].Value ?? string.Empty,
                                    StatementLine61EntryDate = tag61Match.Groups[nameof(statementLine61EntryDate)].Value ?? string.Empty,
                                    StatementLine61FundsCode = tag61Match.Groups[nameof(statementLine61FundsCode)].Value ?? string.Empty,
                                    StatementLine61Suppliment = tag61Match.Groups[nameof(statementLine61Suppliment)].Value ?? string.Empty,
                                    StatementLine61TrnsactnTypeID = tag61Match.Groups[nameof(statementLine61TrnsactnTypeID)].Value ?? string.Empty,
                                    StatementLine61ValueDate = tag61Match.Groups[nameof(statementLine61ValueDate)].Value.ToDate(),
                                    MT940 = mt940
                                });
                            }

                            #endregion

                        }
                        #endregion

                    }


                    #region Tag 62 and 64
                    var tag62And64Pattern = $"{closingBalance62FOrMHeader}{closingBalance62FDOrC}{closingBalance62FDate}{closingBalance62FCurrency}{closingBalance62FAmount}{closingBalance62FCrLf}{availableBalance64Header}{availableBalance64DOrC}{availableBalance64Date}{availableBalance64Currency}{availableBalance64Amount}{availableBalance64CrLf}";


                    var tag62 = message.Substring(message.IndexOf(":62"));

                    var tag64 = message.Substring(message.IndexOf(":64"));


                    var tag62And64Match = Regex.Match($"{tag62}{tag64}", tag62And64Pattern);
                    if (tag62And64Match.Success)
                    {
                        mt940.ClosingBalance62FAmount = tag62And64Match.Groups[nameof(closingBalance62FAmount)].Value.ToMoney();

                        mt940.ClosingBalance62FCurrency = tag62And64Match.Groups[nameof(closingBalance62FCurrency)].Value ?? string.Empty;

                        mt940.ClosingBalance62FDate = tag62And64Match.Groups[nameof(closingBalance62FDate)].Value.ToDate();

                        mt940.ClosingBalance62FDOrC = tag62And64Match.Groups[nameof(closingBalance62FDOrC)].Value ?? string.Empty;


                        mt940.AvailableBalance64Amount = tag62And64Match.Groups[nameof(availableBalance64Amount)].Value.ToMoney();

                        mt940.AvailableBalance64Currency = tag62And64Match.Groups[nameof(availableBalance64Currency)].Value ?? string.Empty;

                        mt940.AvailableBalance64Date = tag62And64Match.Groups[nameof(availableBalance64Date)].Value.ToDate();

                        mt940.AvailableBalance64DOrC = tag62And64Match.Groups[nameof(availableBalance64DOrC)].Value ?? string.Empty;

                    }

                    #endregion


                    //save here

                    applicationDbContext.MT940s.Add(mt940);
                    if (tag61And86Groups.Count > 1) applicationDbContext.Tag61And86Groups.AddRange(tag61And86Groups);

                    await applicationDbContext.SaveChangesAsync();

                }

            }
            catch (Exception exc)
            {
                return null;
            }


            if (mt940 != null && mt940.AccountID25 != null)

                return new ParseMT940WithStatusCRT
                {
                    MT940CRTs = new()
                    {
                        new MT940CRT
                        {
                            MT940MonoCRT = new MT940MonoCRT
                            {
                                ID = mt940.ID,
                                AccountID25 = mt940.AccountID25,
                                AvailableBalance64Amount = mt940.AvailableBalance64Amount,
                                AvailableBalance64Date = mt940.AvailableBalance64Date,
                                AvailableBalance64Currency = mt940.AvailableBalance64Currency,
                                AvailableBalance64DOrC = mt940.AvailableBalance64DOrC,
                                ClosingBalance62FAmount = mt940.ClosingBalance62FAmount,
                                ClosingBalance62FCurrency = mt940.ClosingBalance62FCurrency,
                                ClosingBalance62FDate = mt940.ClosingBalance62FDate,
                                ClosingBalance62FDOrC = mt940.ClosingBalance62FDOrC,
                                FinSwiftAddress = mt940.FinSwiftAddress,
                                FinBranchCode = mt940.FinBranchCode,
                                FinLTCode = mt940.FinLTCode,
                                OpeningBalance60FAmount = mt940.OpeningBalance60FAmount,
                                OpeningBalance60FCurrency = mt940.OpeningBalance60FCurrency,
                                OpeningBalance60FDate = mt940.OpeningBalance60FDate,
                                OpeningBalance60FDOrC = mt940.OpeningBalance60FDOrC,
                                StatementOrSeqNo28CMsgSeq = mt940.StatementOrSeqNo28CMsgSeq,
                                SendersSwiftAddress = mt940.SendersSwiftAddress,
                                SequenceNumber = mt940.SequenceNumber,
                                SessionNumber = mt940.SessionNumber,
                                StatementOrSeqNo28CStmntSeq = mt940.StatementOrSeqNo28CStmntSeq,
                                TransactionRefNo20 = mt940.TransactionRefNo20
                            },
                            Tag61And86GroupCRTs = tag61And86Groups.Select(n => new Tag61And86GroupCRT
                            {
                                ID = n.ID,
                                MT940ID = n.MT940ID,
                                AccOwnerInfo86Info = n.AccOwnerInfo86Info,
                                StatementLine61Amount = n.StatementLine61Amount,
                                StatementLine61BankRef = n.StatementLine61BankRef,
                                StatementLine61CustomerRef = n.StatementLine61CustomerRef,
                                StatementLine61DOrC = n.StatementLine61DOrC,
                                StatementLine61EntryDate = n.StatementLine61EntryDate,
                                StatementLine61FundsCode = n.StatementLine61FundsCode,
                                StatementLine61Suppliment = n.StatementLine61Suppliment,
                                StatementLine61TrnsactnTypeID = n.StatementLine61TrnsactnTypeID,
                                StatementLine61ValueDate = n.StatementLine61ValueDate
                            }).ToList()
                        }
                    },
                    SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = string.Empty, Success = true }
                };

            return new ParseMT940WithStatusCRT
            {
                MT940CRTs = new(),
                SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.UnreadableFileErrorMessage, Success = false }
            };
        }
        #endregion


        #region Parse Multiple MT940s
        public async Task<ParseMT940WithStatusCRT> ParseMultipleMt940sAsync(ApplicationDbContext applicationDbContext, ApplicationUser applicationUser, IDossier dossierService, List<ApiUploadFileAttachMVM> apiUploadFileAttachMVMs, List<IFormFile> formFiles, string rootFolderLocation)
        {
            List<MT940CRT> mT940CRTs = new();

            var hasAnyWrongFileFormat = false;

            foreach (var formfile in formFiles)
            {
                using (var ms = new MemoryStream())
                {
                    await formfile.CopyToAsync(ms);

                    if (!StaticVariables.IsValidFileExtension(".txt", ms.ToArray()))
                    {
                        hasAnyWrongFileFormat = true;
                        break;
                    }
                }
            }

            if (hasAnyWrongFileFormat) return new ParseMT940WithStatusCRT
            {
                MT940CRTs = new(),
                SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = StaticVariables.UnreadableFileErrorMessage, Success = false }
            };

            var userFiles = await dossierService.SaveFilesAsync(applicationDbContext, applicationUser, apiUploadFileAttachMVMs, formFiles, rootFolderLocation);

            for (int userFileIndex = 0; userFileIndex < userFiles.Count; userFileIndex++)
            {
                var parseResult = await (this as IMessageManager).ParseMT940(applicationDbContext, applicationUser, userFiles[userFileIndex].FileUri);

                mT940CRTs.AddRange(parseResult.MT940CRTs);

            }

            return new ParseMT940WithStatusCRT
            {
                SuccessStatusMessageCRT = new SuccessStatusMessageCRT { StatusMessage = string.Empty, Success = true },
                MT940CRTs = mT940CRTs
            };
        }
        #endregion

    }
}
