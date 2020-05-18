using System;
using System.Collections.Generic;
using System.Text;

namespace Pact.App.Core.Models
{
  using Hl7.Fhir.Model;

  public class ConditionDetail
    {
      private Condition Condition { get; }

      public ConditionDetail(Condition condition)
      {
        this.Condition = condition;
      }

      public string Date => this.Condition.AssertedDate;

      public string Display => this.Condition.Code.Text;
  }
}
