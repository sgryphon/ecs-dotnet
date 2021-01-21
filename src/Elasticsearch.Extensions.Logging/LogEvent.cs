// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elastic.CommonSchema;

namespace Elasticsearch.Extensions.Logging
{
	public class LogEvent : Base
	{
		// Custom fields; use capitalisation as per ECS
		private const string MessageTemplatePropertyName = nameof(MessageTemplate);
		private const string ScopesPropertyName = nameof(Scopes);

		// Use name forward compatible with ECS 1.6
		private const string SpanPropertyName = "span";

		/// <summary>
		/// Custom field with the original template used to generate the message, with token placeholders
		/// for inserted label values, e.g. "Unexpected error processing customer {CustomerId}."
		/// </summary>
		public string? MessageTemplate { get; set; }

		/// <summary>
		/// Custom field with an array of string formatted scope values, in the order added.
		/// </summary>
		public IList<string>? Scopes { get; set; }

		/// <summary>
		/// Holds the ID of the current span.
		/// </summary>
		public Span? Span { get; set; }

		/// If <see cref="TryRead" /> returns <c>true</c> this will be called with the deserialized <paramref name="value" />
		/// </summary>
		/// <param name="propertyName">The additional property <see cref="BaseJsonConverter" /> encountered</param>
		/// <param name="value">
		/// The deserialized boxed value you will have to manually unbox to the type that
		/// <see cref="TryRead" /> set
		/// </param>
		/// <returns></returns>
		protected override bool ReceiveProperty(string propertyName, object value) =>
			propertyName switch
			{
				MessageTemplatePropertyName => null != (MessageTemplate = value as string),
				ScopesPropertyName => null != (Scopes = value as IList<string>),
				SpanPropertyName => null != (Span = value as Span),
				_ => false
			};

		/// <summary>
		/// If implemented in a subclass, this allows you to hook into <see cref="BaseJsonConverter" />
		/// and make it aware of properties on a subclass of <see cref="Base" />.
		/// If <paramref name="propertyName" /> is known, set <paramref name="type" /> to the correct type and return true.
		/// </summary>
		/// <param name="propertyName">The additional property that <see cref="BaseJsonConverter" /> encountered</param>
		/// <param name="type">Set this to the type you wish to deserialize to</param>
		/// <returns>Return true if <paramref name="propertyName" /> is handled</returns>
		protected override bool TryRead(string propertyName, out Type? type)
		{
			type = propertyName switch
			{
				MessageTemplatePropertyName => typeof(string),
				ScopesPropertyName => typeof(IList<string>),
				SpanPropertyName => typeof(Span),
				_ => null
			};
			return type != null;
		}

		/// <summary>
		/// Write any additional properties in your subclass during <see cref="BaseJsonConverter" /> serialization.
		/// </summary>
		/// <param name="write">An action taking a <c>property name</c> and <c>boxed value</c> to write to the output</param>
		protected override void WriteAdditionalProperties(Action<string, object> write)
		{
			if (MessageTemplate != null) write(MessageTemplatePropertyName, MessageTemplate);
			if (Scopes != null) write(ScopesPropertyName, Scopes);
			if (Span != null) write(SpanPropertyName, Span);
		}
	}

	public class Span
	{
		/// <summary>
		/// Unique identifier of the span.<para/>
		/// </summary>
		/// <example>e7bc32771d164a92</example><example>a0177b7435d7d545</example>
		[DataMember(Name = "id")]
		public string Id { get; set; } = string.Empty;
	}
}
