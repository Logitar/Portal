import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";

export function getCustomAttributeModifications(source: CustomAttribute[], destination: CustomAttribute[]): CustomAttributeModification[] {
  const modifications: CustomAttributeModification[] = [];

  const destinationKeys = new Set(destination.map(({ key }) => key));
  for (const customAttribute of source) {
    const key = customAttribute.key;
    if (!destinationKeys.has(key)) {
      modifications.push({ key });
    }
  }

  const sourceMap = new Map(source.map(({ key, value }) => [key, value]));
  for (const customAttribute of destination) {
    if (sourceMap.get(customAttribute.key) !== customAttribute.value) {
      modifications.push(customAttribute);
    }
  }

  return modifications;
}
