<script setup lang="ts">
import { computed, inject, ref, watchEffect } from "vue";
import { useForm } from "vee-validate";
import BirthdateInput from "@/components/users/BirthdateInput.vue";
import GenderSelect from "@/components/users/GenderSelect.vue";
import PersonNameInput from "@/components/users/PersonNameInput.vue";
import PictureInput from "@/components/users/PictureInput.vue";
import ProfileInput from "@/components/users/ProfileInput.vue";
import TimeZoneSelect from "@/components/users/TimeZoneSelect.vue";
import WebsiteInput from "@/components/users/WebsiteInput.vue";
import type { UpdateUserPayload } from "@/types/users/payloads";
import type { User, UserUpdatedEvent } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { saveProfile } from "@/api/account";
import { updateUser } from "@/api/users";

const handleError = inject(handleErrorKey) as (e: unknown) => void;

const props = withDefaults(
  defineProps<{
    isProfile?: boolean;
    user: User;
  }>(),
  {
    isProfile: false,
  }
);

const birthdate = ref<Date>();
const firstName = ref<string>("");
const gender = ref<string>("");
const lastName = ref<string>("");
const locale = ref<string>("");
const middleName = ref<string>("");
const nickname = ref<string>("");
const picture = ref<string>("");
const profile = ref<string>("");
const timeZone = ref<string>("");
const website = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const user: User | undefined = props.user;
  if (!user) {
    return false;
  }
  return (
    firstName.value !== (user.firstName ?? "") ||
    lastName.value !== (user.lastName ?? "") ||
    middleName.value !== (user.middleName ?? "") ||
    nickname.value !== (user.nickname ?? "") ||
    birthdate.value?.getTime() !== (user.birthdate ? new Date(user.birthdate) : undefined)?.getTime() ||
    gender.value !== (user.gender ?? "") ||
    locale.value !== (user.locale ?? "") ||
    timeZone.value !== (user.timeZone ?? "") ||
    picture.value !== (user.picture ?? "") ||
    profile.value !== (user.profile ?? "") ||
    website.value !== (user.website ?? "")
  );
});

watchEffect(() => {
  const user = props.user;
  birthdate.value = user.birthdate ? new Date(user.birthdate) : undefined;
  firstName.value = user.firstName ?? "";
  gender.value = user.gender ?? "";
  lastName.value = user.lastName ?? "";
  locale.value = user.locale ?? "";
  middleName.value = user.middleName ?? "";
  nickname.value = user.nickname ?? "";
  picture.value = user.picture ?? "";
  profile.value = user.profile ?? "";
  timeZone.value = user.timeZone ?? "";
  website.value = user.website ?? "";
});

const emit = defineEmits<{
  (e: "user-updated", event: UserUpdatedEvent): void;
}>();
const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const reference = props.user;
    const payload: UpdateUserPayload = {
      firstName: firstName.value !== reference.firstName ? { value: firstName.value } : undefined,
      middleName: middleName.value !== reference.middleName ? { value: middleName.value } : undefined,
      lastName: lastName.value !== reference.lastName ? { value: lastName.value } : undefined,
      nickname: nickname.value !== reference.nickname ? { value: nickname.value } : undefined,
      birthdate: birthdate.value !== reference.birthdate ? { value: birthdate.value } : undefined,
      gender: gender.value !== reference.gender ? { value: gender.value } : undefined,
      locale: locale.value !== reference.locale ? { value: locale.value } : undefined,
      timeZone: timeZone.value !== reference.timeZone ? { value: timeZone.value } : undefined,
      picture: picture.value !== reference.picture ? { value: picture.value } : undefined,
      profile: profile.value !== reference.profile ? { value: profile.value } : undefined,
      website: website.value !== reference.website ? { value: website.value } : undefined,
    };
    const user = props.isProfile ? await saveProfile(payload) : await updateUser(props.user.id, payload);
    emit("user-updated", { user });
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div class="mb-3">
      <icon-submit :disabled="!hasChanges || isSubmitting" icon="fas fa-floppy-disk" :loading="isSubmitting" text="actions.save" />
    </div>
    <div class="row">
      <PersonNameInput class="col-lg-6" type="first" validate v-model="firstName" />
      <PersonNameInput class="col-lg-6" type="last" validate v-model="lastName" />
    </div>
    <div class="row">
      <PersonNameInput class="col-lg-6" type="middle" validate v-model="middleName" />
      <PersonNameInput class="col-lg-6" type="nick" validate v-model="nickname" />
    </div>
    <div class="row">
      <BirthdateInput class="col-lg-6" v-model="birthdate" />
      <GenderSelect class="col-lg-6" v-model="gender" />
    </div>
    <div class="row">
      <locale-select class="col-lg-6" placeholder="users.locale.placeholder" v-model="locale" />
      <TimeZoneSelect class="col-lg-6" v-model="timeZone" />
    </div>
    <PictureInput v-model="picture" />
    <ProfileInput v-model="profile" />
    <WebsiteInput v-model="website" />
  </form>
</template>
