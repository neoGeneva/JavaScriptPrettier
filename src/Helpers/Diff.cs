using System;
using System.Collections.Generic;
using DiffPlex;
using DiffPlex.DiffBuilder.Model;
using DiffPlex.Model;

namespace JavaScriptPrettier.Helpers
{
    // Copied from https://github.com/mmanela/diffplex/blob/ee5e83c970b292a3a93c91844843630b686d71fd/DiffPlex/DiffBuilder/InlineDiffBuilder.cs
    // because it's hard coded to ignore whitespace
    internal static class Diff
    {
        public static DiffPaneModel BuildDiffModel(string oldText, string newText)
        {
            if (oldText == null) throw new ArgumentNullException(nameof(oldText));
            if (newText == null) throw new ArgumentNullException(nameof(newText));

            var differ = new Differ();
            var model = new DiffPaneModel();
            DiffResult diffResult = differ.CreateLineDiffs(oldText, newText, ignoreWhitespace: false);
            BuildDiffPieces(diffResult, model.Lines);
            return model;
        }

        private static void BuildDiffPieces(DiffResult diffResult, List<DiffPiece> pieces)
        {
            int bPos = 0;

            foreach (DiffBlock diffBlock in diffResult.DiffBlocks)
            {
                for (; bPos < diffBlock.InsertStartB; bPos++)
                    pieces.Add(new DiffPiece(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));

                int i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                    pieces.Add(new DiffPiece(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted));

                i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                {
                    pieces.Add(new DiffPiece(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
                    bPos++;
                }

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
                {
                    for (; i < diffBlock.DeleteCountA; i++)
                        pieces.Add(new DiffPiece(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted));
                }
                else
                {
                    for (; i < diffBlock.InsertCountB; i++)
                    {
                        pieces.Add(new DiffPiece(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
                        bPos++;

                    }
                }
            }

            for (; bPos < diffResult.PiecesNew.Length; bPos++)
                pieces.Add(new DiffPiece(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));
        }
    }
}
