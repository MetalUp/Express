import { PropertyMember, EntryType, DomainObjectRepresentation } from "@nakedobjects/restful-objects";
import { ICodeUserView, CodeUserView } from "../models/code-user-view";
import { IHintUserView, HintUserView } from "../models/hint-user-view";
import { ITaskUserView, TaskUserView } from "../models/task-user-view";

function getChoicesValue(member: PropertyMember) {
    const raw = member.value().scalar() as number;
    const choices = member.choices();
    for (const k in choices) {
      const v = choices[k];
      if (v.scalar() === raw) {
        return k;
      }
    }
    return undefined;
  }

  function setPropertyValue(toObj: any, member: PropertyMember) {
    const attachmentLink = member.attachmentLink();
    if (attachmentLink) {
      const href = attachmentLink.href();
      const mt = attachmentLink.type().asString;
      toObj[member.id()] = [href, mt];
    }
    else if (member.entryType() == EntryType.Choices) {
      toObj[member.id()] = getChoicesValue(member);
    }
    else if (member.isScalar()) {
      toObj[member.id()] = member.value().scalar();
    }
    else {
      toObj[member.id()] = member.value().getHref();
    }
  }

  export function convertTo<T>(toObj: T, rep: DomainObjectRepresentation) {
    if (rep && Object.keys(rep.propertyMembers()).length > 0) {
      const pMembers = rep.propertyMembers()
      for (const k in pMembers) {
        const member = pMembers[k];
        setPropertyValue(toObj, member);
      }
    }
    return toObj;
  }