<script setup lang="ts">
import { computed, inject, ref, watchEffect } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import AddressLocalityInput from "./AddressLocalityInput.vue";
import AddressStreetTextarea from "./AddressStreetTextarea.vue";
import CountrySelect from "./CountrySelect.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import PhoneExtensionInput from "@/components/users/PhoneExtensionInput.vue";
import PhoneNumberInput from "@/components/users/PhoneNumberInput.vue";
import PostalCodeInput from "@/components/users/PostalCodeInput.vue";
import RegionSelect from "@/components/users/RegionSelect.vue";
import type { AddressPayload, EmailPayload, PhonePayload } from "@/types/users/payloads";
import type { CountrySettings } from "@/types/users";
import type { ProfileUpdatedEvent, User } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { saveProfile } from "@/api/account";

const { t } = useI18n();
const handleError = inject(handleErrorKey) as (e: unknown) => void;

const props = defineProps<{
  user: User;
}>();

const address = ref<AddressPayload>({ street: "", locality: "", region: "", postalCode: "", country: "", isVerified: false });
const email = ref<EmailPayload>({ address: "", isVerified: false });
const phone = ref<PhonePayload>({ countryCode: "", number: "", extension: "", isVerified: false });
const phoneNumberRef = ref<InstanceType<typeof PhoneNumberInput> | null>(null);
const selectedCountry = ref<CountrySettings>();

const hasAddressChanged = computed<boolean>(() => {
  const user = props.user;
  return (
    address.value.street !== (user.address?.street ?? "") ||
    address.value.locality !== (user.address?.locality ?? "") ||
    address.value.postalCode !== (user.address?.postalCode ?? "") ||
    address.value.country !== (user.address?.country ?? "") ||
    address.value.region !== (user.address?.region ?? "")
  );
});
const hasEmailChanged = computed<boolean>(() => email.value.address !== (props.user.email?.address ?? ""));
const hasPhoneChanged = computed<boolean>(() => {
  const user = props.user;
  return (
    (phone.value.countryCode ?? "CA") !== (user.phone?.countryCode ?? "CA") ||
    (phone.value.number ?? "") !== (user.phone?.number ?? "") ||
    phone.value.extension !== (user.phone?.extension ?? "")
  );
});
const hasChanges = computed<boolean>(() => hasAddressChanged.value || hasEmailChanged.value || hasPhoneChanged.value);

const isAddressRequired = computed<boolean>(() => {
  return (
    Boolean(address.value.street) ||
    Boolean(address.value.locality) ||
    Boolean(address.value.postalCode) ||
    Boolean(address.value.country) ||
    Boolean(address.value.region)
  );
});

watchEffect(() => {
  const user = props.user;
  address.value = {
    street: user.address?.street ?? "",
    locality: user.address?.locality ?? "",
    region: user.address?.region ?? "",
    postalCode: user.address?.postalCode ?? "",
    country: user.address?.country ?? "",
    isVerified: user.address?.isVerified ?? false,
  };
  email.value = {
    address: user.email?.address ?? "",
    isVerified: user.email?.isVerified ?? false,
  };
  phone.value = {
    countryCode: user.phone?.countryCode ?? "CA",
    number: user.phone?.number ?? "",
    extension: user.phone?.extension ?? "",
    isVerified: user.phone?.isVerified ?? false,
  };
});

const emit = defineEmits<{
  (e: "profile-updated", event: ProfileUpdatedEvent): void;
}>();
const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const user = await saveProfile({
      address: hasAddressChanged.value ? { value: address.value.street ? address.value : undefined } : undefined,
      email: hasEmailChanged.value ? { value: email.value.address ? email.value : undefined } : undefined,
      phone: hasPhoneChanged.value ? { value: phone.value.number ? phone.value : undefined } : undefined,
    });
    emit("profile-updated", { user });
  } catch (e: unknown) {
    handleError(e);
  }
});

function clearAddress(): void {
  address.value.street = "";
  address.value.locality = "";
  address.value.postalCode = undefined;
  address.value.country = "";
  address.value.region = undefined;
}
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="mb-3">
        <icon-submit :disabled="!hasChanges || isSubmitting" icon="fas fa-floppy-disk" :loading="isSubmitting" text="actions.save" />
      </div>
      <EmailAddressInput :disabled="user.email?.isVerified" validate :verified="user.email?.isVerified" v-model="email.address" />
      <div class="row">
        <PhoneNumberInput
          class="col-lg-6"
          :country-code="phone.countryCode"
          :disabled="user.phone?.isVerified"
          ref="phoneNumberRef"
          :required="Boolean(phone.extension)"
          :verified="user.phone?.isVerified"
          v-model="phone.number"
          @country-code="phone.countryCode = $event"
        />
        <PhoneExtensionInput class="col-lg-6" :disabled="user.phone?.isVerified" validate v-model="phone.extension" />
      </div>
      <h5>
        {{ t("users.address.title") }}
        <template v-if="user.address?.isVerified">
          <app-badge>{{ t("users.address.verified") }}</app-badge>
        </template>
      </h5>
      <AddressStreetTextarea :disabled="user.address?.isVerified" :required="isAddressRequired" validate v-model="address.street" />
      <div class="row">
        <AddressLocalityInput class="col-lg-6" :disabled="user.address?.isVerified" :required="isAddressRequired" validate v-model="address.locality" />
        <PostalCodeInput class="col-lg-6" :country="selectedCountry" :disabled="user.address?.isVerified" validate v-model="address.postalCode" />
      </div>
      <div class="row">
        <CountrySelect
          class="col-lg-6"
          :disabled="user.address?.isVerified"
          :required="isAddressRequired"
          v-model="address.country"
          @country-selected="selectedCountry = $event"
        />
        <RegionSelect class="col-lg-6" :country="selectedCountry" :disabled="user.address?.isVerified" v-model="address.region" />
      </div>
      <icon-button
        v-if="user.address?.isVerified !== true"
        :disabled="!isAddressRequired"
        icon="fas fa-times"
        text="actions.clear"
        variant="warning"
        @click="clearAddress"
      />
    </form>
  </div>
</template>
