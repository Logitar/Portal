<template>
  <b-container>
    <h1 v-t="'dictionaries.title'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" :href="createUrl" icon="plus" text="actions.create" variant="success" />
    </div>
    <b-row>
      <locale-select class="col" v-model="locale" />
      <realm-select class="col" v-model="realm" />
      <sort-select class="col" :isDescending="isDescending" :options="sortOptions" v-model="sort" @isDescending="isDescending = $event" />
      <count-select class="col" v-model="limit" />
    </b-row>
    <template v-if="dictionaries.length">
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'dictionaries.sort.options.RealmLocale'" />
            <th scope="col" v-t="'dictionaries.sort.options.Entries'" />
            <th scope="col" v-t="'dictionaries.sort.options.UpdatedOn'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="dictionary in dictionaries" :key="dictionary.id">
            <td>
              <b-link :href="`/dictionaries/${dictionary.id}`">
                {{ dictionary.realm.displayName ?? dictionary.realm.uniqueName }} | {{ dictionary.locale }}
              </b-link>
            </td>
            <td v-text="dictionary.entries.length" />
            <td><status-cell :actor="dictionary.updatedBy" :date="dictionary.updatedOn" /></td>
            <td>
              <icon-button icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`delete_${dictionary.id}`" />
              <delete-modal
                confirm="dictionaries.delete.confirm"
                :displayName="`${dictionary.realm.displayName ?? dictionary.realm.uniqueName} | ${dictionary.locale}`"
                :id="`delete_${dictionary.id}`"
                :loading="loading"
                title="dictionaries.delete.title"
                @ok="onDelete(dictionary, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="limit" aria-controls="table" />
    </template>
    <p v-else v-t="'dictionaries.empty'" />
  </b-container>
</template>

<script>
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { deleteDictionary, getDictionaries } from '@/api/dictionaries'
import { getQueryString } from '@/helpers/queryUtils'

export default {
  name: 'DictionaryList',
  components: {
    RealmSelect
  },
  data() {
    return {
      dictionaries: [],
      isDescending: false,
      limit: 10,
      loading: false,
      locale: null,
      page: 1,
      realm: null,
      sort: 'RealmLocale',
      total: 0
    }
  },
  computed: {
    createUrl() {
      return '/create-dictionary' + getQueryString({ locale: this.locale, realm: this.realm })
    },
    params() {
      return {
        realm: this.realm,
        locale: this.locale,
        sort: this.sort,
        isDescending: this.isDescending,
        skip: (this.page - 1) * this.limit,
        limit: this.limit
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('dictionaries.sort.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  },
  methods: {
    async onDelete({ id }, callback = null) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await deleteDictionary(id)
          refresh = true
          this.toast('success', 'dictionaries.delete.success')
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
      if (callback) {
        callback()
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getDictionaries(params ?? this.params)
          this.dictionaries = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(newValue, oldValue) {
        if (newValue?.skip && oldValue && (newValue.locale !== oldValue.locale || newValue.realm !== oldValue.realm || newValue.limit !== oldValue.limit)) {
          this.page = 1
          await this.refresh()
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
