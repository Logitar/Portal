<template>
  <div>
    <h3 v-t="'user.information.personal'" />
    <table class="table table-striped">
      <tbody>
        <tr v-if="profile.fullName">
          <th scope="row" v-t="'user.fullName'" />
          <td v-text="profile.fullName" />
        </tr>
        <tr v-if="profile.email">
          <th scope="row" v-t="'user.email.label'" />
          <td>
            {{ profile.email }}
            <b-badge v-if="profile.isEmailConfirmed" variant="info">{{ $t('user.email.confirmed') }}</b-badge>
          </td>
        </tr>
        <tr v-if="profile.phoneNumber">
          <th scope="row" v-t="'user.phone.label'" />
          <td>
            {{ profile.phoneNumber }}
            <b-badge v-if="profile.isPhoneNumberConfirmed" variant="info">{{ $t('user.phone.confirmed') }}</b-badge>
          </td>
        </tr>
        <tr>
          <th scope="row" v-t="'user.createdAt'" />
          <td>{{ $d(new Date(profile.createdAt), 'medium') }}</td>
        </tr>
        <tr v-if="profile.updatedAt">
          <th scope="row" v-t="'user.updatedAt'" />
          <td>{{ $d(new Date(profile.updatedAt), 'medium') }}</td>
        </tr>
        <tr v-if="profile.signedInAt">
          <th scope="row" v-t="'user.signedInAt'" />
          <td>
            {{ $d(new Date(profile.signedInAt), 'medium') }}
            <br />
            <b-link :href="`/sessions?user=${profile.id}`">{{ $t('user.session.view') }}</b-link>
          </td>
        </tr>
      </tbody>
    </table>
    <p v-if="profile.isEmailConfirmed || profile.isPhoneNumberConfirmed" class="text-warning">
      <font-awesome-icon icon="exclamation-triangle" /> <i v-t="'user.confirmed.warning'" />
    </p>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <b-row>
          <email-field class="col" :confirmed="profile.isEmailConfirmed" validate v-model="email" />
          <phone-field class="col" :confirmed="profile.isPhoneNumberConfirmed" validate v-model="phoneNumber" />
        </b-row>
        <b-row>
          <first-name-field class="col" validate v-model="firstName" />
          <last-name-field class="col" validate v-model="lastName" />
        </b-row>
        <b-row>
          <locale-select class="col" v-model="locale" />
          <picture-field class="col" validate v-model="picture" />
        </b-row>
        <div class="mb-2">
          <icon-submit :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
        </div>
      </b-form>
    </validation-observer>
  </div>
</template>

<script>
import EmailField from '@/components/User/EmailField.vue'
import FirstNameField from '@/components/User/FirstNameField.vue'
import LastNameField from '@/components/User/LastNameField.vue'
import PhoneField from '@/components/User/PhoneField.vue'
import PictureField from '@/components/User/PictureField.vue'
import { saveProfile } from '@/api/account'

export default {
  name: 'PersonalInformation',
  components: {
    EmailField,
    FirstNameField,
    LastNameField,
    PhoneField,
    PictureField
  },
  props: {
    profile: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      email: null,
      firstName: null,
      lastName: null,
      loading: false,
      locale: null,
      middleName: null,
      phoneNumber: null,
      picture: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.email ?? '') !== (this.profile.email ?? '') ||
        (this.phoneNumber ?? '') !== (this.profile.phoneNumber ?? '') ||
        (this.firstName ?? '') !== (this.profile.firstName ?? '') ||
        (this.lastName ?? '') !== (this.profile.lastName ?? '') ||
        this.locale !== this.profile.locale ||
        (this.picture ?? '') !== (this.profile.picture ?? '')
      )
    },
    payload() {
      return {
        email: this.email || null,
        phoneNumber: this.phoneNumber || null,
        firstName: this.firstName,
        lastName: this.lastName,
        middleName: this.middleName,
        locale: this.locale,
        picture: this.picture || null
      }
    }
  },
  methods: {
    setModel(profile) {
      this.email = profile.email
      this.firstName = profile.firstName
      this.lastName = profile.lastName
      this.locale = profile.locale
      this.middleName = profile.middleName
      this.phoneNumber = profile.phoneNumber
      this.picture = profile.picture
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await saveProfile(this.payload)
            this.setModel(data)
            this.toast('success', 'user.profile.updated')
            this.$refs.form.reset()
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
    profile: {
      deep: true,
      immediate: true,
      handler(profile) {
        this.setModel(profile)
      }
    }
  }
}
</script>
