﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Monitorian.Core.Helper;

namespace Monitorian.Core.Models
{
	internal class OperationRecorder
	{
		private const string OperationOption = "/operation";

		public static IReadOnlyCollection<string> Options => new[] { OperationOption };

		private static bool IsRecording() => Environment.GetCommandLineArgs().Skip(1).Contains(OperationOption);

		public static OperationRecorder Create() => IsRecording() ? new OperationRecorder() : null;

		private OperationRecorder() => LogService.RecordOperation(nameof(Create));

		public void Record(string content) => LogService.RecordOperation(content);

		private string _actionName;

		public void StartRecord(string actionName) => this._actionName = actionName;

		private readonly List<(string group, string item)> _groups = new List<(string, string)>();

		public void AddItem(string groupName, string itemString) => _groups.Add((groupName, itemString));
		public void AddItems(string groupName, IEnumerable<string> itemStrings) => _groups.AddRange(itemStrings.Select(x => (groupName, x)));

		public void StopRecord()
		{
			var groupsStrings = _groups.GroupBy(x => x.group).Select(x => (x.Key, (object)x.Select(y => new StringWrapper(y.item)))).ToArray();

			LogService.RecordOperation($"{_actionName}{Environment.NewLine}{SimpleSerialization.Serialize(groupsStrings)}");

			_groups.Clear();
		}
	}
}