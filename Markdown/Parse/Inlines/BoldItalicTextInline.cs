﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System.Collections.Generic;

namespace Discord_UWP.MarkdownTextBlock.Parse.Inlines
{
    /// <summary>
    /// Represents a span containing bold italic text.
    /// </summary>
    internal class BoldItalicTextInline : MarkdownInline, IInlineContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoldItalicTextInline"/> class.
        /// </summary>
        public BoldItalicTextInline()
            : base(MarkdownInlineType.Bold)
        {
        }

        /// <summary>
        /// Gets or sets the contents of the inline.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        internal static void AddTripChars(List<Helpers.Common.InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new Helpers.Common.InlineTripCharHelper() { FirstChar = '*', Method = Helpers.Common.InlineParseMethod.BoldItalic });
            tripCharHelpers.Add(new Helpers.Common.InlineTripCharHelper() { FirstChar = '_', Method = Helpers.Common.InlineParseMethod.BoldItalic });
        }

        /// <summary>
        /// Attempts to parse a bold text span.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed bold text span, or <c>null</c> if this is not a bold text span. </returns>
        internal static Helpers.Common.InlineParseResult Parse(string markdown, int start, int maxEnd)
        {
            if (start >= maxEnd - 3)
            {
                return null;
            }

            if (markdown == null || markdown.Length < 6)
            {
                return null;
            }

            // Check the start sequence.
            string startSequence = markdown.Substring(start, 3);
            if (startSequence != "***" && startSequence != "___")
            {
                return null;
            }

            // Find the end of the span.  The end sequence (either '***' or '___') must be the same
            // as the start sequence.
            var innerStart = start + 3;
            int innerEnd = Helpers.Common.IndexOf(markdown, startSequence, innerStart, maxEnd);
            if (innerEnd == -1)
            {
                return null;
            }

            // The span must contain at least one character.
            if (innerStart == innerEnd)
            {
                return null;
            }

            // The first character inside the span must NOT be a space.
            if (Helpers.Common.IsWhiteSpace(markdown[innerStart]))
            {
                return null;
            }

            // The last character inside the span must NOT be a space.
            if (Helpers.Common.IsWhiteSpace(markdown[innerEnd - 1]))
            {
                return null;
            }

            // We found something!
            var bold = new BoldTextInline
            {
                Inlines = new List<MarkdownInline>
                {
                    new ItalicTextInline
                    {
                        Inlines = Helpers.Common.ParseInlineChildren(markdown, innerStart, innerEnd)
                    }
                }
            };
            return new Helpers.Common.InlineParseResult(bold, start, innerEnd + 3);
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null)
            {
                return base.ToString();
            }

            return "***" + string.Join(string.Empty, Inlines) + "***";
        }
    }
}