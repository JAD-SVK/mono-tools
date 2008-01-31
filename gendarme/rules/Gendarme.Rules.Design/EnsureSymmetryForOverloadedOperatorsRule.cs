//
// Gendarme.Rules.Design.EnsureSymmetryForOverloadedOperatorsRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//
//  (C) 2008 Andreas Noever
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Design {

	public class EnsureSymmetryForOverloadedOperatorsRule : ITypeRule {

		static KeyValuePair<MethodSignature, MethodSignature> [] SymmetricOperators_Warning = new KeyValuePair<MethodSignature, MethodSignature> [] { 
			new KeyValuePair<MethodSignature,MethodSignature> (MethodSignatures.op_Addition, MethodSignatures.op_Subtraction),
			new KeyValuePair<MethodSignature,MethodSignature> (MethodSignatures.op_Multiply, MethodSignatures.op_Division),
			new KeyValuePair<MethodSignature,MethodSignature> (MethodSignatures.op_Division, MethodSignatures.op_Modulus),
		};
		static KeyValuePair<MethodSignature, MethodSignature> [] SymmetricOperators_Error = new KeyValuePair<MethodSignature, MethodSignature> [] { 
			new KeyValuePair<MethodSignature,MethodSignature> (MethodSignatures.op_GreaterThan, MethodSignatures.op_LessThan),
			new KeyValuePair<MethodSignature,MethodSignature> (MethodSignatures.op_GreaterThanOrEqual, MethodSignatures.op_LessThanOrEqual),
			new KeyValuePair<MethodSignature, MethodSignature> (MethodSignatures.op_Equality, MethodSignatures.op_Inequality),
			new KeyValuePair<MethodSignature, MethodSignature> (MethodSignatures.op_True, MethodSignatures.op_False),
		};

		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			if (type.IsInterface || type.IsEnum)
				return runner.RuleSuccess;

			MessageCollection results = null;

			foreach (var kv in SymmetricOperators_Warning)
				CheckOperatorPair (kv, type, MessageType.Warning, ref results);
			foreach (var kv in SymmetricOperators_Error)
				CheckOperatorPair (kv, type, MessageType.Error, ref results);

			return results;
		}

		private static void CheckOperatorPair (KeyValuePair<MethodSignature, MethodSignature> pair, TypeDefinition type, 
			MessageType msgType, ref MessageCollection results)
		{
			MethodDefinition op = type.GetMethod (pair.Key);
			if (op == null) { //first one not defined
				pair = new KeyValuePair<MethodSignature, MethodSignature> (pair.Value, pair.Key); //reverse
				op = type.GetMethod (pair.Key);
				if (op == null)
					return; //neither one is defined
			} else {
				if (type.HasMethod (pair.Value))
					return; //both are defined
			}
			if (results == null)
				results = new MessageCollection ();
			Location loc = new Location (op);
			string s = string.Format ("This type implements the '{0}' operator so, for symmetry, it should also implement the '{1}' operator.", 
				pair.Key.Name, pair.Value.Name);
			Message msg = new Message (s, loc, msgType);
			results.Add (msg);
		}
	}
}
