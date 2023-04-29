<template>
  <div>
    <h3 v-t="'user.information.personal'" />
    <table class="table table-striped">
      <tbody>
        <tr v-if="user.fullName">
          <th scope="row" v-t="'user.fullName'" />
          <td v-text="user.fullName" />
        </tr>
        <tr v-if="user.email">
          <th scope="row" v-t="'user.email.label'" />
          <td>
            {{ user.email.address }}
            <b-badge v-if="user.email.isVerified" variant="info">{{ $t('user.email.verified') }}</b-badge>
          </td>
        </tr>
        <tr v-if="user.phone">
          <th scope="row" v-t="'user.phone.e164'" />
          <td>
            {{ user.phone.e164Formatted }}
            <b-badge v-if="user.phone.isVerified" variant="info">{{ $t('user.phone.verified') }}</b-badge>
          </td>
        </tr>
        <tr>
          <th scope="row" v-t="'user.createdOn'" />
          <td>{{ $d(new Date(user.createdOn), 'medium') }}</td>
        </tr>
        <tr v-if="user.updatedOn">
          <th scope="row" v-t="'user.updatedOn'" />
          <td>{{ $d(new Date(user.updatedOn), 'medium') }}</td>
        </tr>
        <tr v-if="user.signedInOn">
          <th scope="row" v-t="'user.signedInOn'" />
          <td>
            {{ $d(new Date(user.signedInOn), 'medium') }}
            <br />
            <b-link :href="`/sessions?user=${user.id}`">{{ $t('user.session.view') }}</b-link>
          </td>
        </tr>
      </tbody>
    </table>
    <p v-if="user.isConfirmed" class="text-warning"><font-awesome-icon icon="exclamation-triangle" /> <i v-t="'user.verifiedWarning'" /></p>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <b-row>
          <email-field class="col" validate :verified="user.email?.isVerified" v-model="email.address" />
        </b-row>
        <b-row>
          <phone-field class="col" ref="phone" :required="Boolean(phone.extension)" :verified="user.phone?.isVerified" v-model="phone" />
          <phone-extension-field class="col" validate v-model="phone.extension" />
        </b-row>
        <b-row>
          <first-name-field class="col" validate v-model="firstName" />
          <last-name-field class="col" validate v-model="lastName" />
        </b-row>
        <b-row>
          <middle-name-field class="col" validate v-model="middleName" />
          <nickname-field class="col" validate v-model="nickname" />
        </b-row>
        <b-row>
          <birthdate-field class="col" v-model="birthdate" />
          <gender-select class="col" v-model="gender" />
        </b-row>
        <b-row>
          <locale-select class="col" v-model="locale" />
          <timezone-select class="col" v-model="timeZone" />
        </b-row>
        <b-row>
          <picture-field class="col" validate v-model="picture" />
          <profile-field class="col" validate v-model="profile" />
        </b-row>
        <b-row>
          <website-field class="col" validate v-model="website" />
        </b-row>
        <div class="mb-2">
          <icon-submit :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
        </div>
      </b-form>
    </validation-observer>
  </div>
</template>

<script>
import Vue from 'vue'
import BirthdateField from '@/components/User/BirthdateField.vue'
import EmailField from '@/components/User/EmailField.vue'
import FirstNameField from '@/components/User/FirstNameField.vue'
import GenderSelect from '@/components/User/GenderSelect.vue'
import LastNameField from '@/components/User/LastNameField.vue'
import MiddleNameField from '@/components/User/MiddleNameField.vue'
import NicknameField from '@/components/User/NicknameField.vue'
import PhoneExtensionField from '@/components/User/PhoneExtensionField.vue'
import PhoneField from '@/components/User/PhoneField.vue'
import PictureField from '@/components/User/PictureField.vue'
import ProfileField from '@/components/User/ProfileField.vue'
import WebsiteField from '@/components/User/WebsiteField.vue'
import { updateUser } from '@/api/users'

export default {
  name: 'PersonalInformation',
  components: {
    BirthdateField,
    EmailField,
    FirstNameField,
    GenderSelect,
    LastNameField,
    MiddleNameField,
    NicknameField,
    PhoneExtensionField,
    PhoneField,
    PictureField,
    ProfileField,
    WebsiteField
  },
  props: {
    user: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      birthdate: null,
      email: {
        address: null
      },
      firstName: null,
      gender: null,
      lastName: null,
      loading: false,
      locale: null,
      middleName: null,
      nickname: null,
      phone: {
        countryCode: null,
        extension: null,
        number: null
      },
      picture: null,
      profile: null,
      timeZone: null,
      website: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.email.address ?? '') !== (this.user?.email?.address ?? '') ||
        (this.phone.number && (this.phone.countryCode ?? '') !== (this.user?.phone?.countryCode ?? '')) ||
        (this.phone.number ?? '') !== (this.user?.phone?.number ?? '') ||
        (this.phone.extension ?? '') !== (this.user?.phone?.extension ?? '') ||
        (this.firstName ?? '') !== (this.user?.firstName ?? '') ||
        (this.middleName ?? '') !== (this.user?.middleName ?? '') ||
        (this.lastName ?? '') !== (this.user?.lastName ?? '') ||
        (this.nickname ?? '') !== (this.user?.nickname ?? '') ||
        (this.birthdate ?? '') !== (this.user?.birthdate ?? '') ||
        (this.gender ?? '') !== (this.user?.gender ?? '') ||
        (this.locale ?? '') !== (this.user?.locale ?? '') ||
        (this.timeZone ?? '') !== (this.user?.timeZone ?? '') ||
        (this.picture ?? '') !== (this.user?.picture ?? '') ||
        (this.profile ?? '') !== (this.user?.profile ?? '') ||
        (this.website ?? '') !== (this.user?.website ?? '')
      )
    },
    payload() {
      return {
        address: this.user?.address ?? null,
        email: this.email.address ? { ...this.email } : null,
        phone: this.phone.number ? { ...this.phone } : null,
        firstName: this.firstName,
        middleName: this.middleName,
        lastName: this.lastName,
        nickname: this.nickname,
        birthdate: this.birthdate,
        gender: this.gender,
        locale: this.locale,
        timeZone: this.timeZone,
        picture: this.picture,
        profile: this.profile,
        website: this.website,
        customAttributes: this.user?.customAttributes ?? null
      }
    }
  },
  methods: {
    setModel(user) {
      this.birthdate = user.birthdate
      Vue.set(this.email, 'address', user.email?.address ?? null)
      this.firstName = user.firstName
      this.gender = user.gender
      this.lastName = user.lastName
      this.locale = user.locale
      this.middleName = user.middleName
      this.nickname = user.nickname
      Vue.set(this.phone, 'countryCode', user.phone?.countryCode ?? null)
      Vue.set(this.phone, 'number', user.phone?.number ?? null)
      Vue.set(this.phone, 'extension', user.phone?.extension ?? null)
      this.picture = user.picture
      this.profile = user.profile
      this.timeZone = user.timeZone
      this.website = user.website
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if ((await this.$refs.form.validate()) && this.$refs.phone.isValid) {
            const { data } = await updateUser(this.user.id, this.payload)
            this.$refs.form.reset()
            this.$emit('updated', data)
            this.toast('success', 'user.profile.updated')
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    user: {
      deep: true,
      immediate: true,
      handler(user) {
        this.setModel(user)
      }
    }
  }
}
</script>
