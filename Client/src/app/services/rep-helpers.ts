import { PropertyMember, EntryType, DomainObjectRepresentation, DomainServicesRepresentation, IHateoasModel, InvokableActionMember } from "@nakedobjects/restful-objects";
import { ContextService, RepLoaderService } from "@nakedobjects/services";


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

// eslint-disable-next-line @typescript-eslint/no-explicit-any
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

export function convertTo<T>(toObj: T, rep?: DomainObjectRepresentation) {
  if (rep && Object.keys(rep.propertyMembers()).length > 0) {
    const pMembers = rep.propertyMembers();
    for (const k in pMembers) {
      const member = pMembers[k];
      setPropertyValue(toObj, member);
    }
  }
  return toObj;
}

export function getService(contextService: ContextService, repLoader: RepLoaderService, name: string) {
  return contextService.getServices()
    .then((services: DomainServicesRepresentation) => {
      const service = services.getService(name);
      return repLoader.populate(service);
    })
    .then((s: IHateoasModel) => s as DomainObjectRepresentation);
}

export function getAction(getService: () => Promise<DomainObjectRepresentation>, name: string) {
  return getService().then(service => service.actionMember(name) as InvokableActionMember);
}