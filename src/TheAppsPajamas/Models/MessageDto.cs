using System;
using TheAppsPajamas.Shared.Types;

namespace TheAppsPajamas.Models
{
    public class MessageDto
    {
        public int MessageImportanceTypeValue { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }

        public MessageDto()
        {
        }

        public MessageDto(int messageImportanceTypeValue, string messageBody, string messageTitle = null)
        {
            MessageImportanceTypeValue = messageImportanceTypeValue;
            MessageTitle = messageTitle;
            MessageBody = messageBody;
            MessageTitle = String.IsNullOrEmpty(messageTitle) ? MessageImportanceType.FromValue(messageImportanceTypeValue).DisplayName : messageTitle;
        }

        public MessageDto(MessageImportanceType messageImportanceType, string messageBody, string messageTitle = null)
        {
            MessageImportanceTypeValue = messageImportanceType.Value;
            MessageBody = messageBody;
            MessageTitle = String.IsNullOrEmpty(messageTitle) ? messageImportanceType.DisplayName : messageTitle;
        }
    }
}
