<template>
  <b-container>
    <h1 v-t="user ? 'user.editTitle' : 'user.newTitle'" />
    <template v-if="user">
      <status-detail :model="user" />
      <p v-if="user.signedInOn">
        {{ $t('user.signedInOn') }} {{ $d(new Date(user.signedInOn), 'medium') }}
        <br />
        <b-link :href="viewSessionsUrl">{{ $t('user.session.view') }}</b-link>
      </p>
      <p v-if="user.isDisabled" class="text-danger"><status-info :actor="user.disabledBy" :date="new Date(user.disabledOn)" dateFormat="user.disabledOn" /></p>
    </template>
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <template v-if="user">
            <icon-submit class="mx-1" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
            <toggle-status :disabled="user.id === current" :user="user" @updated="setModel" />
          </template>
          <icon-submit class="mx-1" v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <realm-select :disabled="Boolean(user)" :required="!user" v-model="realmId" />
        <h3 v-t="'user.information.authentication'" />
        <b-alert dismissible variant="warning" v-model="usernameConflict">
          <strong v-t="'user.username.conflict'" />
        </b-alert>
        <username-field
          :disabled="Boolean(user)"
          placeholder="user.create.usernamePlaceholder"
          ref="username"
          :required="!user"
          :settings="selectedRealm?.usernameSettings"
          :validate="!user"
          v-model="username"
        />
        <template v-if="!user || user.passwordChangedOn">
          <h5 v-if="user" v-t="'user.password.label'" />
          <p v-if="user && user.passwordChangedOn">{{ $t('user.password.changedOn') }} {{ $d(new Date(user.passwordChangedOn), 'medium') }}</p>
          <p v-if="user && (password || passwordConfirmation)" class="text-warning">
            <font-awesome-icon icon="exclamation-triangle" /> <i v-t="'user.password.warning'" />
          </p>
          <b-row>
            <password-field
              v-if="user"
              class="col"
              label="user.password.new.label"
              placeholder="user.password.new.placeholder"
              :settings="selectedRealm?.passwordSettings"
              validate
              v-model="password"
            />
            <password-field
              v-else
              class="col"
              placeholder="user.create.passwordPlaceholder"
              required
              :settings="selectedRealm?.passwordSettings"
              validate
              v-model="password"
            />
            <password-field
              class="col"
              id="confirm"
              label="user.password.confirm.label"
              placeholder="user.password.confirm.placeholder"
              :required="!user || Boolean(password)"
              :rules="{ confirmed: 'password' }"
              v-model="passwordConfirmation"
            />
          </b-row>
        </template>
        <h3 v-t="'user.information.personal'" />
        <b-alert dismissible variant="warning" v-model="emailConflict">
          <strong v-t="'user.email.conflict.header'" />
          <template v-if="selectedRealm">{{ ` ${$t('user.email.conflict.detail', { name: selectedRealm.name })}` }}</template>
        </b-alert>
        <p v-if="user?.isConfirmed" class="text-warning"><font-awesome-icon icon="exclamation-triangle" /> <i v-t="'user.verifiedWarning'" /></p>
        <b-row>
          <email-field class="col" :verified="user?.email?.isVerified" ref="email" validate v-model="emailAddress" />
          <phone-field class="col" :verified="user?.phone?.isVerified" validate v-model="phoneNumber" />
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
        <h3 v-t="'user.externalIdentifiers.title'" />
        <table class="table table-striped" v-if="user && user.externalIdentifiers.length">
          <thead>
            <tr>
              <th scope="col" v-t="'user.externalIdentifiers.key'" />
              <th scope="col" v-t="'user.externalIdentifiers.value'" />
              <th scope="col" v-t="'user.externalIdentifiers.updatedOn'" />
            </tr>
          </thead>
          <tbody>
            <tr v-for="externalIdentifier in user.externalIdentifiers" :key="externalIdentifier.id">
              <td>
                {{ externalIdentifier.key }}
                <font-awesome-icon v-if="externalIdentifier.key === 'Google'" :icon="['fab', 'google']" />
              </td>
              <td v-text="externalIdentifier.value" />
              <td><status-cell :actor="externalIdentifier.updatedBy" :date="externalIdentifier.updatedOn" /></td>
            </tr>
          </tbody>
        </table>
        <p v-else v-t="'user.externalIdentifiers.empty'" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import BirthdateField from './BirthdateField.vue'
import EmailField from './EmailField.vue'
import FirstNameField from './FirstNameField.vue'
import GenderSelect from './GenderSelect.vue'
import LastNameField from './LastNameField.vue'
import MiddleNameField from './MiddleNameField.vue'
import NicknameField from './NicknameField.vue'
import PasswordField from './PasswordField.vue'
import PhoneField from './PhoneField.vue'
import PictureField from './PictureField.vue'
import ProfileField from './ProfileField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import ToggleStatus from './ToggleStatus.vue'
import UsernameField from './UsernameField.vue'
import WebsiteField from './WebsiteField.vue'
import { createUser, updateUser } from '@/api/users'
import { getQueryString } from '@/helpers/queryUtils'
import { getRealm } from '@/api/realms'

export default {
  name: 'UserEdit',
  components: {
    BirthdateField,
    EmailField,
    FirstNameField,
    GenderSelect,
    LastNameField,
    MiddleNameField,
    NicknameField,
    PasswordField,
    PhoneField,
    PictureField,
    ProfileField,
    RealmSelect,
    ToggleStatus,
    UsernameField,
    WebsiteField
  },
  props: {
    current: {
      type: String,
      default: ''
    },
    json: {
      type: String,
      default: ''
    },
    realm: {
      type: String,
      default: ''
    },
    status: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      birthdate: null,
      currentUser: null,
      emailAddress: null,
      emailConflict: null,
      firstName: null,
      gender: null,
      lastName: null,
      loading: false,
      locale: null,
      middleName: null,
      nickname: null,
      password: null,
      passwordConfirmation: null,
      phoneNumber: null,
      picture: null,
      profile: null,
      realmId: null,
      selectedRealm: null,
      timeZone: null,
      user: null,
      username: null,
      usernameConflict: false,
      website: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (!this.user && this.realmId) ||
        (!this.user && this.username) ||
        this.password ||
        this.passwordConfirmation ||
        (this.emailAddress ?? '') !== (this.user?.email?.address ?? '') ||
        (this.phoneNumber ?? '') !== (this.user?.phone?.number ?? '') ||
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
      const payload = {
        password: this.password || null,
        address: this.user?.address ?? null,
        email: this.emailAddress ? { address: this.emailAddress } : null,
        phone: this.phoneNumber
          ? {
              countryCode: this.user?.phone?.countryCode ?? null,
              number: this.phoneNumber,
              extension: this.user?.phone?.extension ?? null
            }
          : null,
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
      if (!this.user) {
        payload.realm = this.realmId
        payload.username = this.username
      }
      return payload
    },
    viewSessionsUrl() {
      return '/sessions' + getQueryString({ realm: this.user.realm?.id ?? null, user: this.user.id })
    }
  },
  methods: {
    setModel(user) {
      this.user = user
      this.birthdate = user.birthdate
      this.emailAddress = user.email?.address ?? null
      this.firstName = user.firstName
      this.gender = user.gender
      this.lastName = user.lastName
      this.locale = user.locale
      this.middleName = user.middleName
      this.nickname = user.nickname
      this.password = null
      this.passwordConfirmation = null
      this.phoneNumber = user.phone?.number ?? null
      this.picture = user.picture
      this.profile = user.profile
      this.realmId = user.realm?.id ?? null
      this.selectedRealm ??= user.realm
      this.timeZone = user.timeZone
      this.username = user.username
      this.website = user.website
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        this.emailConflict = false
        this.usernameConflict = false
        try {
          if (await this.$refs.form.validate()) {
            if (this.user) {
              const { data } = await updateUser(this.user.id, this.payload)
              this.setModel(data)
              this.toast('success', 'user.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createUser(this.payload)
              window.location.replace(`/users/${data.id}?status=created`)
            }
          }
        } catch (e) {
          const { data, status } = e
          if (status === 409) {
            if (data?.code === 'UniqueNameAlreadyUsed') {
              this.usernameConflict = true
              this.$refs.username.focus()
              return
            } else if (data?.code === 'EmailAddressAlreadyUsed') {
              this.emailConflict = true
              this.$refs.email.focus()
              return
            }
          }
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.realm) {
      this.realmId = this.realm
    }
    if (this.status === 'created') {
      this.toast('success', 'user.created')
    }
  },
  watch: {
    async realmId(id) {
      if (id) {
        try {
          const { data } = await getRealm(id)
          this.selectedRealm = data
        } catch (e) {
          this.handleError(e)
        }
      } else {
        this.selectedRealm = null
      }
    }
  }
}
</script>
