// 
// Gendarme.Rules.Design.FinalizersShouldBeProtectedRule
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//
// Copyright (C) 2008 Daniel Abramov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Design {

	public class FinalizersShouldBeProtectedRule : ITypeRule {

		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner)
		{
			MethodDefinition finalizer = typeDefinition.GetFinalizer ();

			if (finalizer == null) // no finalizer found
				return runner.RuleSuccess;

			// good finalizer:
			if (finalizer.IsFamily && !finalizer.IsFamilyAndAssembly && !finalizer.IsFamilyOrAssembly)
				return runner.RuleSuccess;

			Location loc = new Location (finalizer);
			Message msg = new Message ("Finalizer must be protected in order not to be called from user code.", loc, MessageType.Error);
			return new MessageCollection (msg);
		}
	}
}
