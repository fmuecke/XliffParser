namespace xlflib
{
    public class XlfState
    {
        public enum Predefined
        {
            Final,                  //   Indicates the terminating state.
            NeedsAdaptation,        //    Indicates only non-textual information needs adaptation.
            NeedsL10n,              //  Indicates both text and non-textual information needs adaptation.
            NeedsReviewAdaptation,  //     Indicates only non-textual information needs review.
            NeedsReviewL10n,        //   Indicates both text and non-textual information needs review.
            NeedsReviewTranslation, //    Indicates that only the text of the item needs to be reviewed.
            NeedsTranslation,       //   Indicates that the item needs to be translated.
            New,                    // Indicates that the item is new. For example, translation units that were not in a previous version of the document.
            SignedOff,              // Indicates that changes are reviewed and approved.
            Translated,             // Indicates that the item has been translated.
        }

        bool IsUserDefined {
            get;
        }
    }
}